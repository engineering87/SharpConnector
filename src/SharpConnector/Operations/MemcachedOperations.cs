// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpConnector.Configuration;
using SharpConnector.Connectors.Memcached;
using SharpConnector.Entities;

namespace SharpConnector.Operations
{
    public class MemcachedOperations<T> : Operations<T>
    {
        private readonly MemcachedWrapper _memcachedWrapper;

        /// <summary>
        /// Create a new MemcachedOperations instance.
        /// </summary>
        public MemcachedOperations(MemcachedConfig memcachedConfig)
        {
            _memcachedWrapper = new MemcachedWrapper(memcachedConfig);
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override T Get(string key)
        {
            var connectorEntity = _memcachedWrapper.Get(key);
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
            var connectorEntity = _memcachedWrapper.GetAsync(key);
            if (connectorEntity != null)
                return (Task<T>)Convert.ChangeType(connectorEntity.Result.Payload, typeof(Task<T>));
            return default;
        }

        public override IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
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
            return _memcachedWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <returns></returns>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _memcachedWrapper.Insert(connectorEntity);
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
            return _memcachedWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <returns></returns>
        public override Task<bool> InsertAsync(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _memcachedWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override bool Delete(string key)
        {
            return _memcachedWrapper.Delete(key);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override Task<bool> DeleteAsync(string key)
        {
            return _memcachedWrapper.DeleteAsync(key);
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
            return _memcachedWrapper.Update(connectorEntity);
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
            return _memcachedWrapper.UpdateAsync(connectorEntity);
        }
    }
}
