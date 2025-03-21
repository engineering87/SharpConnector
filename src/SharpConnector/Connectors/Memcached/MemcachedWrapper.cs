// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Newtonsoft.Json;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.Memcached
{
    public class MemcachedWrapper
    {
        private readonly MemcachedAccess _memcachedAccess;

        /// <summary>
        /// Create a new MemcachedWrapper instance.
        /// </summary>
        /// <param name="memcachedConfig">The Memcached connector config.</param>
        public MemcachedWrapper(MemcachedConfig memcachedConfig)
        {
            _memcachedAccess = new MemcachedAccess(memcachedConfig);
        }

        private int GetExpiration(ConnectorEntity connectorEntity)
        {
            return connectorEntity.Expiration?.Seconds ?? 0;
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public ConnectorEntity Get(string key)
        {
            return _memcachedAccess.MemcachedClient.Get<ConnectorEntity>(key);
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public async Task<ConnectorEntity> GetAsync(string key)
        {
            var result = await _memcachedAccess.MemcachedClient.GetAsync<ConnectorEntity>(key);
            return result?.Value;
        }

        /// <summary>
        /// Get all ConnectorEntities asynchronously.
        /// </summary>
        /// <returns>A list of ConnectorEntities.</returns>
        public async Task<List<ConnectorEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public bool Insert(ConnectorEntity connectorEntity)
        {
            var seconds = connectorEntity.Expiration?.Seconds ?? 0;
            return _memcachedAccess.MemcachedClient.Add(connectorEntity.Key,
                JsonConvert.SerializeObject(connectorEntity),
                seconds);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public Task<bool> InsertAsync(ConnectorEntity connectorEntity)
        {
            var seconds = connectorEntity.Expiration?.Seconds ?? 0;
            return _memcachedAccess.MemcachedClient.AddAsync(connectorEntity.Key,
                JsonConvert.SerializeObject(connectorEntity),
                seconds);
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="values">The ConnectorEntities to store.</param>
        /// <returns></returns>
        public bool InsertMany(List<ConnectorEntity> connectorEntities)
        {
            return connectorEntities.All(Insert);
        }

        /// <summary>
        /// Asynchronously inserts multiple ConnectorEntities.
        /// </summary>
        /// <param name="connectorEntities">The list of ConnectorEntities to store.</param>
        /// <returns>True if all operations succeeded, false otherwise.</returns>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities)
        {
            var tasks = connectorEntities.Select(InsertAsync);
            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            return _memcachedAccess.MemcachedClient.Remove(key);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(string key)
        {
            return _memcachedAccess.MemcachedClient.RemoveAsync(key);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public bool Update(ConnectorEntity connectorEntity)
        {
            var seconds = connectorEntity.Expiration?.Seconds ?? 0;
            return _memcachedAccess.MemcachedClient.Replace(connectorEntity.Key,
                JsonConvert.SerializeObject(connectorEntity),
                seconds);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(ConnectorEntity connectorEntity)
        {
            var seconds = connectorEntity.Expiration?.Seconds ?? 0;
            return _memcachedAccess.MemcachedClient.ReplaceAsync(connectorEntity.Key,
                JsonConvert.SerializeObject(connectorEntity),
                seconds);
        }

        /// <summary>
        /// Check if a Key exists.
        /// </summary>
        public bool Exists(string key)
        {
            var value = _memcachedAccess.MemcachedClient.Get<object>(key);
            return value != null;
        }

        /// <summary>
        /// Check if a Key exists asynchronously.
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            var result = await _memcachedAccess.MemcachedClient.GetAsync<object>(key);
            return result?.Value != null;
        }
    }
}
