// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Collections.Generic;
using Newtonsoft.Json;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System;

namespace SharpConnector.Connectors.Redis
{
    public class RedisWrapper : IDisposable
    {
        private const int ReadBatchSize = 256;
        private readonly RedisAccess _redisAccess;
        private readonly string _keyPrefix;

        /// <summary>
        /// Create a new RedisWrapper instance.
        /// </summary>
        /// <param name="redisConfig">The Redis connector config.</param>
        public RedisWrapper(RedisConfig redisConfig)
        {
            _keyPrefix = redisConfig.KeyPrefix ?? string.Empty;
            _redisAccess = new RedisAccess(redisConfig);
        }

        /// <summary>
        /// Disposes the underlying Redis connection.
        /// </summary>
        public void Dispose()
        {
            _redisAccess?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get a Redis database; keys are transparently prefixed when a KeyPrefix is configured.
        /// </summary>
        private IDatabase GetDatabase(int databaseNumber)
        {
            var database = _redisAccess.Connection.GetDatabase(databaseNumber);
            return _keyPrefix.Length == 0
                ? database
                : database.WithKeyPrefix(_keyPrefix);
        }

        /// <summary>
        /// Get the raw (non-prefixed) database, used with fully-qualified keys returned by SCAN.
        /// </summary>
        private IDatabase GetRawDatabase(int databaseNumber) =>
            _redisAccess.Connection.GetDatabase(databaseNumber);

        /// <summary>
        /// Pattern used to scan keys owned by this connector.
        /// </summary>
        internal static RedisValue BuildScanPattern(string keyPrefix) =>
            string.IsNullOrEmpty(keyPrefix) ? default : keyPrefix + "*";

        /// <summary>
        /// Serialize an object to JSON.
        /// </summary>
        private static string Serialize(object obj) =>
            JsonConvert.SerializeObject(obj);

        /// <summary>
        /// Deserialize JSON to an object.
        /// </summary>
        private static T Deserialize<T>(string json) =>
            JsonConvert.DeserializeObject<T>(json);

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        public ConnectorEntity Get(string key, int databaseNumber = 0)
        {
            var value = GetDatabase(databaseNumber).StringGet(key);
            return value.HasValue ? Deserialize<ConnectorEntity>(value) : null;
        }

        /// <summary>
        /// Get the value of Key asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<ConnectorEntity> GetAsync(string key, int databaseNumber = 0, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var value = await GetDatabase(databaseNumber)
                .StringGetAsync(key)
                .ConfigureAwait(false);

            return value.HasValue ? Deserialize<ConnectorEntity>(value) : null;
        }

        /// <summary>
        /// Get all values from Redis database.
        /// </summary>
        /// <param name="databaseNumber">The Redis database.</param>
        public IEnumerable<ConnectorEntity> GetAll(int databaseNumber = 0)
        {
            var connection = _redisAccess.Connection;
            var database = GetRawDatabase(databaseNumber);
            var pattern = BuildScanPattern(_keyPrefix);
            var entities = new List<ConnectorEntity>();

            foreach (var endpoint in connection.GetEndPoints())
            {
                var server = connection.GetServer(endpoint);
                // Skip replicas to avoid duplicated entries in clustered/replicated setups.
                if (server.IsReplica)
                    continue;

                var keys = server.Keys(databaseNumber, pattern);

                foreach (var key in keys)
                {
                    RedisValue value;
                    try
                    {
                        value = database.StringGet(key);
                    }
                    catch (RedisServerException ex) when (IsWrongType(ex))
                    {
                        // Key holds a non-string value not produced by SharpConnector; skip it.
                        continue;
                    }

                    if (!value.HasValue)
                        continue;

                    var entity = TryDeserialize(value);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            return entities;
        }

        /// <summary>
        /// Asynchronously get all values from Redis database.
        /// </summary>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation, containing a list of all ConnectorEntity instances in the specified database.</returns>
        public async Task<IEnumerable<ConnectorEntity>> GetAllAsync(int databaseNumber = 0, CancellationToken ct = default)
        {
            var connection = _redisAccess.Connection;
            var database = GetRawDatabase(databaseNumber);
            var pattern = BuildScanPattern(_keyPrefix);
            var entities = new List<ConnectorEntity>();

            foreach (var endpoint in connection.GetEndPoints())
            {
                ct.ThrowIfCancellationRequested();

                var server = connection.GetServer(endpoint);
                // Skip replicas to avoid duplicated entries in clustered/replicated setups.
                if (server.IsReplica)
                    continue;

                var batch = new List<Task<ConnectorEntity>>(ReadBatchSize);
                await foreach (var key in server.KeysAsync(databaseNumber, pattern).WithCancellation(ct).ConfigureAwait(false))
                {
                    batch.Add(GetEntityAsync(database, key, ct));
                    if (batch.Count < ReadBatchSize)
                        continue;

                    await FlushBatchAsync(batch, entities).ConfigureAwait(false);
                }

                await FlushBatchAsync(batch, entities).ConfigureAwait(false);
            }

            return entities;
        }

        private static async Task FlushBatchAsync(List<Task<ConnectorEntity>> batch, List<ConnectorEntity> entities)
        {
            if (batch.Count == 0)
                return;

            var results = await Task.WhenAll(batch).ConfigureAwait(false);
            entities.AddRange(results.Where(e => e != null));
            batch.Clear();
        }

        private static async Task<ConnectorEntity> GetEntityAsync(IDatabase database, RedisKey key, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var value = await database.StringGetAsync(key).ConfigureAwait(false);
                return value.HasValue ? TryDeserialize(value) : null;
            }
            catch (RedisServerException ex) when (IsWrongType(ex))
            {
                // Key holds a non-string value not produced by SharpConnector; skip it.
                return null;
            }
        }

        /// <summary>
        /// Returns true when the Redis error indicates an operation against a key holding the wrong kind of value.
        /// </summary>
        internal static bool IsWrongType(RedisServerException ex) =>
            ex?.Message != null && ex.Message.StartsWith("WRONGTYPE", StringComparison.Ordinal);

        /// <summary>
        /// Attempts to deserialize a value into a <see cref="ConnectorEntity"/>.
        /// Returns <c>null</c> when the value is not a valid ConnectorEntity payload.
        /// </summary>
        internal static ConnectorEntity TryDeserialize(RedisValue value)
        {
            try
            {
                return Deserialize<ConnectorEntity>(value);
            }
            catch (JsonException)
            {
                // The value was not produced by SharpConnector (or the schema changed); ignore.
                return null;
            }
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        public bool Insert(ConnectorEntity connectorEntity, int databaseNumber = 0)
        {
            return GetDatabase(databaseNumber).StringSet(
                connectorEntity.Key,
                Serialize(connectorEntity),
                ToExpiration(connectorEntity.Expiration));
        }

        /// <summary>
        /// Asynchronously sets the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> InsertAsync(ConnectorEntity connectorEntity, int databaseNumber = 0, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            return await GetDatabase(databaseNumber).StringSetAsync(
                connectorEntity.Key,
                Serialize(connectorEntity),
                ToExpiration(connectorEntity.Expiration))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously sets multiple keys.
        /// </summary>
        /// <param name="connectorEntities">The ConnectorEntities to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities, int databaseNumber = 0, CancellationToken ct = default)
        {
            var database = GetDatabase(databaseNumber);
            var tasks = new List<Task<bool>>();

            foreach (var entity in connectorEntities)
            {
                ct.ThrowIfCancellationRequested();
                tasks.Add(database.StringSetAsync(entity.Key, JsonConvert.SerializeObject(entity), ToExpiration(entity.Expiration)));
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            return results.All(result => result);
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="connectorEntities">The ConnectorEntities to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        public bool InsertMany(List<ConnectorEntity> connectorEntities, int databaseNumber = 0)
        {
            var result = true;
            var database = GetDatabase(databaseNumber);
            foreach (var entity in connectorEntities)
            {
                var currentResult = database.StringSet(
                    entity.Key,
                    JsonConvert.SerializeObject(entity),
                    ToExpiration(entity.Expiration));

                if (currentResult == false)
                    result = false;
            }
            return result;
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        public bool Delete(string key, int databaseNumber = 0)
        {
            return GetDatabase(databaseNumber).KeyDelete(key);
        }

        /// <summary>
        /// Removes the specified Key asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> DeleteAsync(string key, int databaseNumber = 0, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await GetDatabase(databaseNumber).KeyDeleteAsync(key).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to update.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        public bool Update(ConnectorEntity connectorEntity, int databaseNumber = 0)
        {
            return GetDatabase(databaseNumber).StringSet(
                connectorEntity.Key,
                Serialize(connectorEntity),
                ToExpiration(connectorEntity.Expiration),
                When.Exists);
        }

        /// <summary>
        /// Asynchronously updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to update.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> UpdateAsync(ConnectorEntity connectorEntity, int databaseNumber = 0, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            return await GetDatabase(databaseNumber).StringSetAsync(
                connectorEntity.Key,
                Serialize(connectorEntity),
                ToExpiration(connectorEntity.Expiration),
                When.Exists)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public bool Exists(string key, int databaseNumber = 0)
        {
            return GetDatabase(databaseNumber).KeyExists(key);
        }

        /// <summary>
        /// Asynchronously checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task containing true if the item exists, false otherwise.</returns>
        public async Task<bool> ExistsAsync(string key, int databaseNumber = 0, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await GetDatabase(databaseNumber).KeyExistsAsync(key).ConfigureAwait(false);
        }

        /// <summary>
        /// Queries Redis database with a filter function.
        /// </summary>
        /// <param name="filter">A function to filter the results.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns>A collection of filtered ConnectorEntity instances.</returns>
        public IEnumerable<ConnectorEntity> Query(Func<ConnectorEntity, bool> filter, int databaseNumber = 0)
        {
            return GetAll(databaseNumber).Where(filter);
        }

        /// <summary>
        /// Asynchronously queries Redis database with a filter function.
        /// </summary>
        /// <param name="filter">A function to filter the results.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task containing a collection of filtered ConnectorEntity instances.</returns>
        public async Task<IEnumerable<ConnectorEntity>> QueryAsync(Func<ConnectorEntity, bool> filter, int databaseNumber = 0, CancellationToken ct = default)
        {
            var allEntities = await GetAllAsync(databaseNumber, ct).ConfigureAwait(false);
            return allEntities.Where(filter);
        }

        #region Internal Utilities

        /// <summary>
        /// Converts a nullable <see cref="TimeSpan"/> value to an <see cref="Expiration"/> instance.
        /// </summary>
        /// <param name="expiration">The optional expiration interval to convert. If <see langword="null"/>, a default expiration is returned.</param>
        /// <returns>An <see cref="Expiration"/> representing the specified interval, or the default value if <paramref
        /// name="expiration"/> is <see langword="null"/>.</returns>
        private static Expiration ToExpiration(TimeSpan? expiration)
        {
            if (!expiration.HasValue)
                return default;
            return new Expiration(expiration.Value);
        }

        #endregion
    }
}