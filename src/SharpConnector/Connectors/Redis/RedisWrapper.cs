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
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public ConnectorEntity Get(string key, int databaseNumber = 0)
        {
            var database = _redisAccess.GetConnection().GetDatabase(databaseNumber);
            var value = database.StringGet(key);
            return !value.IsNull ? JsonConvert.DeserializeObject<ConnectorEntity>(value) : default;
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public async Task<ConnectorEntity> GetAsync(string key, int databaseNumber = 0)
        {
            var database = _redisAccess.GetConnection().GetDatabase(databaseNumber);
            var value = await database.StringGetAsync(key).ConfigureAwait(false);
            return value.HasValue ? JsonConvert.DeserializeObject<ConnectorEntity>(value) : default;
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
            var result = new List<ConnectorEntity>();
            var connection = _redisAccess.GetConnection();
            var database = connection.GetDatabase(databaseNumber);
            var endpoints = connection.GetEndPoints(true);

            foreach (var endpoint in endpoints)
            {
                var server = connection.GetServer(endpoint);
                var keys = server.Keys(databaseNumber);

                foreach (var redisKey in keys)
                {
                    var value = database.StringGet(redisKey);
                    if (!value.IsNull)
                    {
                        result.Add(JsonConvert.DeserializeObject<ConnectorEntity>(value));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Asynchronously get all values from Redis database.
        /// </summary>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns>A task that represents the asynchronous operation, containing a list of all ConnectorEntity instances in the specified database.</returns>
        public async Task<IEnumerable<ConnectorEntity>> GetAllAsync(int databaseNumber = 0)
        {
            var result = new List<ConnectorEntity>();
            var connection = _redisAccess.GetConnection();
            var database = connection.GetDatabase(databaseNumber);
            var endpoints = connection.GetEndPoints(true);

            foreach (var endpoint in endpoints)
            {
                var server = connection.GetServer(endpoint);
                var keys = server.Keys(databaseNumber);

                var tasks = keys.Select(async redisKey =>
                {
                    var value = await database.StringGetAsync(redisKey);
                    return !value.IsNull ? JsonConvert.DeserializeObject<ConnectorEntity>(value) : null;
                });

                var entities = await Task.WhenAll(tasks);
                result.AddRange(entities.Where(e => e != null));
            }
            return result;
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public bool Insert(ConnectorEntity connectorEntity, int databaseNumber = 0)
        {
            var database = _redisAccess.GetConnection().GetDatabase(databaseNumber);
            return database.StringSet(
                connectorEntity.Key, 
                JsonConvert.SerializeObject(connectorEntity), 
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
            var database = _redisAccess.GetConnection().GetDatabase(databaseNumber);
            return await database.StringSetAsync(
                connectorEntity.Key,
                JsonConvert.SerializeObject(connectorEntity),
                connectorEntity.Expiration).ConfigureAwait(false);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities, int databaseNumber = 0)
        {
            var database = _redisAccess.GetConnection().GetDatabase(databaseNumber);
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
            var database = _redisAccess.GetConnection().GetDatabase(databaseNumber);
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
            var redisKey = new RedisKey(key);
            var database = _redisAccess.GetConnection().GetDatabase(databaseNumber);
            return database.KeyDelete(redisKey);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string key, int databaseNumber = 0)
        {
            var redisKey = new RedisKey(key);
            var database = _redisAccess.GetConnection().GetDatabase(databaseNumber);
            return await database.KeyDeleteAsync(redisKey);
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
