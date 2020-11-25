// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Threading.Tasks;
using SharpConnector.Configuration;
using SharpConnector.Connectors.Redis;
using SharpConnector.Entities;

namespace SharpConnector.Operations
{
    public class RedisOperations<T> : Operations<T>
    {
        private readonly RedisWrapper _redisWrapper;

        /// <summary>
        /// Create a new RedisOperations instance.
        /// </summary>
        /// <param name="redisConfig">The Redis connector config.</param>
        public RedisOperations(RedisConfig redisConfig)
        {
            _redisWrapper = new RedisWrapper(redisConfig);
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override T Get(string key)
        {
            var connectorEntity = _redisWrapper.Get(key);
            if (connectorEntity != null)
                return (T)Convert.ChangeType(connectorEntity.Payload, typeof(T));
            return default;
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override Task<T> GetAsync(string key)
        {
            var connectorEntity = _redisWrapper.GetAsync(key);
            if (connectorEntity != null)
                return (Task<T>)Convert.ChangeType(connectorEntity.Result.Payload, typeof(Task<T>));
            return default;
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _redisWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override Task<bool> InsertAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _redisWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override bool Delete(string key)
        {
            return _redisWrapper.Delete(key);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override Task<bool> DeleteAsync(string key)
        {
            return _redisWrapper.DeleteAsync(key);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _redisWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override Task<bool> UpdateAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _redisWrapper.UpdateAsync(connectorEntity);
        }
    }
}
