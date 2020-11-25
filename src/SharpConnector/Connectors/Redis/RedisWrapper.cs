// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Newtonsoft.Json;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using StackExchange.Redis;
using System.Threading.Tasks;

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
        public Task<ConnectorEntity> GetAsync(string key, int databaseNumber = 0)
        {
            var database = _redisAccess.GetConnection().GetDatabase(databaseNumber);
            var value = database.StringGetAsync(key);
            return value.Result.HasValue ? JsonConvert.DeserializeObject<Task<ConnectorEntity>>(value.Result) : default;
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
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public Task<bool> InsertAsync(ConnectorEntity connectorEntity, int databaseNumber = 0)
        {
            var database = _redisAccess.GetConnection().GetDatabase(databaseNumber);
            return database.StringSetAsync(
                connectorEntity.Key, 
                JsonConvert.SerializeObject(connectorEntity),
                connectorEntity.Expiration);
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
        public Task<bool> DeleteAsync(string key, int databaseNumber = 0)
        {
            var redisKey = new RedisKey(key);
            var database = _redisAccess.GetConnection().GetDatabase(databaseNumber);
            return database.KeyDeleteAsync(redisKey);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public bool Update(ConnectorEntity connectorEntity, int databaseNumber = 0)
        {
            return Insert(connectorEntity, databaseNumber);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <param name="databaseNumber">The Redis database.</param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(ConnectorEntity connectorEntity, int databaseNumber = 0)
        {
            return InsertAsync(connectorEntity, databaseNumber);
        }
    }
}
