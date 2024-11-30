// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Collections.Generic;
using Newtonsoft.Json;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Linq;

namespace SharpConnector.Connectors.Redis
{
    public class RedisWrapper
    {
        private readonly RedisAccess _redisAccess;

        /// <summary>
        /// Create a new RedisWrapper instance.
        /// </summary>
        /// <param name="redisConfig">The Redis connector config.</param>
        public RedisWrapper(RedisConfig redisConfig)
        {
            _redisAccess = new RedisAccess(redisConfig);
        }

        /// <summary>
        /// Get a Redis database.
        /// </summary>
        private IDatabase GetDatabase(int databaseNumber) =>
            _redisAccess.Connection.GetDatabase(databaseNumber);

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
        /// <returns></returns>
        public ConnectorEntity Get(string key, int databaseNumber = 0)
        {
            var value = GetDatabase(databaseNumber).StringGet(key);
            return value.HasValue ? Deserialize<ConnectorEntity>(value) : null;
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public async Task<ConnectorEntity> GetAsync(string key, int databaseNumber = 0)
        {
            var value = await GetDatabase(databaseNumber).StringGetAsync(key).ConfigureAwait(false);
            return value.HasValue ? Deserialize<ConnectorEntity>(value) : null;
        }

        /// <summary>
        /// Get all values from Redis database.
        /// </summary>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        /// <summary>
        /// Get all values from Redis database.
        /// </summary>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public IEnumerable<ConnectorEntity> GetAll(int databaseNumber = 0)
        {
            var connection = _redisAccess.Connection;
            var database = GetDatabase(databaseNumber);
            var entities = new List<ConnectorEntity>();

            foreach (var endpoint in connection.GetEndPoints())
            {
                var server = connection.GetServer(endpoint);
                var keys = server.Keys(databaseNumber);

                foreach (var key in keys)
                {
                    var value = database.StringGet(key);
                    if (value.HasValue)
                    {
                        entities.Add(Deserialize<ConnectorEntity>(value));
                    }
                }
            }
            return entities;
        }

        /// <summary>
        /// Asynchronously get all values from Redis database.
        /// </summary>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns>A task that represents the asynchronous operation, containing a list of all ConnectorEntity instances in the specified database.</returns>
        public async Task<IEnumerable<ConnectorEntity>> GetAllAsync(int databaseNumber = 0)
        {
            var connection = _redisAccess.Connection;
            var database = GetDatabase(databaseNumber);
            var entities = new List<ConnectorEntity>();

            foreach (var endpoint in connection.GetEndPoints())
            {
                var server = connection.GetServer(endpoint);
                var keys = server.Keys(databaseNumber);

                var tasks = keys.Select(async key =>
                {
                    var value = await database.StringGetAsync(key);
                    return value.HasValue ? Deserialize<ConnectorEntity>(value) : null;
                });

                var results = await Task.WhenAll(tasks).ConfigureAwait(false);
                entities.AddRange(results.Where(e => e != null));
            }
            return entities;
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public bool Insert(ConnectorEntity connectorEntity, int databaseNumber = 0)
        {
            return GetDatabase(databaseNumber).StringSet(
                connectorEntity.Key,
                Serialize(connectorEntity),
                connectorEntity.Expiration);
        }

        /// <summary>
        /// Asynchronously sets the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(ConnectorEntity connectorEntity, int databaseNumber = 0)
        {
            return await GetDatabase(databaseNumber).StringSetAsync(
                connectorEntity.Key,
                Serialize(connectorEntity),
                connectorEntity.Expiration);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities, int databaseNumber = 0)
        {
            var database = _redisAccess.Connection.GetDatabase(databaseNumber);
            var tasks = connectorEntities.Select(entity =>
                database.StringSetAsync(entity.Key, JsonConvert.SerializeObject(entity), entity.Expiration));

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            return results.All(result => result);
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="values">The ConnectorEntities to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public bool InsertMany(List<ConnectorEntity> connectorEntities, int databaseNumber = 0)
        {
            var result = true;
            var database = _redisAccess.Connection.GetDatabase(databaseNumber);
            foreach (var entity in connectorEntities)
            {
                var currentResult = database.StringSet(
                    entity.Key,
                    JsonConvert.SerializeObject(entity),
                    entity.Expiration);

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
        /// <returns></returns>
        public bool Delete(string key, int databaseNumber = 0)
        {
            return GetDatabase(databaseNumber).KeyDelete(key);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string key, int databaseNumber = 0)
        {
            return await GetDatabase(databaseNumber).KeyDeleteAsync(key);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to update.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public bool Update(ConnectorEntity connectorEntity, int databaseNumber = 0) =>
            Insert(connectorEntity, databaseNumber);

        /// <summary>
        /// Asynchronously updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to update.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(ConnectorEntity connectorEntity, int databaseNumber = 0) =>
            InsertAsync(connectorEntity, databaseNumber);
    }
}