using Newtonsoft.Json;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System.Collections.Generic;
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
            var result = true;
            foreach (var entity in connectorEntities)
            {
                var seconds = entity.Expiration?.Seconds ?? 0;
                var currentResult = _memcachedAccess.MemcachedClient.Add(entity.Key,
                                JsonConvert.SerializeObject(entity),
                                seconds);

                if (currentResult == false)
                    result = false;
            }
            return result;
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
    }
}
