// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Newtonsoft.Json;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.Memcached
{
    public class MemcachedWrapper
    {
        private readonly MemcachedAccess _memcachedAccess;

        /// <summary>
        /// Create a new MemcachedWrapper instance.
        /// </summary>
        /// <param name="memcachedConfig">The Memcached connector configuration.</param>
        public MemcachedWrapper(MemcachedConfig memcachedConfig)
        {
            _memcachedAccess = new MemcachedAccess(memcachedConfig);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public ConnectorEntity Get(string key)
        {
            var json = _memcachedAccess.MemcachedClient.Get<string>(key);
            return json != null ? JsonConvert.DeserializeObject<ConnectorEntity>(json) : null;
        }

        /// <summary>
        /// Asynchronously retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<ConnectorEntity> GetAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var res = await _memcachedAccess.MemcachedClient
                .GetAsync<string>(key)
                .ConfigureAwait(false);

            var json = res?.Value;
            return json != null ? JsonConvert.DeserializeObject<ConnectorEntity>(json) : null;
        }

        /// <summary>
        /// Asynchronously retrieve all ConnectorEntities.
        /// </summary>
        /// <remarks>
        /// Not supported by Memcached since it does not provide key enumeration.
        /// </remarks>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>Never returns; always throws <see cref="NotSupportedException"/>.</returns>
        public Task<List<ConnectorEntity>> GetAllAsync(CancellationToken ct = default)
        {
            throw new NotSupportedException("Memcached does not support retrieving all keys/values.");
        }

        /// <summary>
        /// Set the key to hold the specified value.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        public bool Insert(ConnectorEntity connectorEntity)
        {
            var seconds = connectorEntity.Expiration.HasValue
                ? (int)Math.Ceiling(connectorEntity.Expiration.Value.TotalSeconds)
                : 0;

            return _memcachedAccess.MemcachedClient.Add(
                connectorEntity.Key,
                JsonConvert.SerializeObject(connectorEntity),
                seconds);
        }

        /// <summary>
        /// Asynchronously set the key to hold the specified value.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public Task<bool> InsertAsync(ConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var seconds = connectorEntity.Expiration.HasValue
                ? (int)Math.Ceiling(connectorEntity.Expiration.Value.TotalSeconds)
                : 0;

            return _memcachedAccess.MemcachedClient.AddAsync(
                connectorEntity.Key,
                JsonConvert.SerializeObject(connectorEntity),
                seconds);
        }

        /// <summary>
        /// Insert multiple entities.
        /// </summary>
        /// <param name="connectorEntities">The entities to store.</param>
        public bool InsertMany(List<ConnectorEntity> connectorEntities)
        {
            return connectorEntities.All(Insert);
        }

        /// <summary>
        /// Asynchronously insert multiple entities.
        /// </summary>
        /// <param name="connectorEntities">The entities to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if all operations succeeded; otherwise, false.</returns>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities, CancellationToken ct = default)
        {
            var tasks = new List<Task<bool>>(connectorEntities.Count);
            foreach (var entity in connectorEntities)
            {
                ct.ThrowIfCancellationRequested();
                tasks.Add(InsertAsync(entity, ct));
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            return results.All(r => r);
        }

        /// <summary>
        /// Remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public bool Delete(string key)
        {
            return _memcachedAccess.MemcachedClient.Remove(key);
        }

        /// <summary>
        /// Asynchronously remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return _memcachedAccess.MemcachedClient.RemoveAsync(key);
        }

        /// <summary>
        /// Update the specified key with the provided value.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        public bool Update(ConnectorEntity connectorEntity)
        {
            var seconds = connectorEntity.Expiration.HasValue
                ? (int)Math.Ceiling(connectorEntity.Expiration.Value.TotalSeconds)
                : 0;

            return _memcachedAccess.MemcachedClient.Replace(
                connectorEntity.Key,
                JsonConvert.SerializeObject(connectorEntity),
                seconds);
        }

        /// <summary>
        /// Asynchronously update the specified key with the provided value.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public Task<bool> UpdateAsync(ConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var seconds = connectorEntity.Expiration.HasValue
                ? (int)Math.Ceiling(connectorEntity.Expiration.Value.TotalSeconds)
                : 0;

            return _memcachedAccess.MemcachedClient.ReplaceAsync(
                connectorEntity.Key,
                JsonConvert.SerializeObject(connectorEntity),
                seconds);
        }

        /// <summary>
        /// Check whether a key exists.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public bool Exists(string key)
        {
            var json = _memcachedAccess.MemcachedClient.Get<string>(key);
            return json != null;
        }

        /// <summary>
        /// Asynchronously check whether a key exists.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var res = await _memcachedAccess.MemcachedClient
                .GetAsync<string>(key)
                .ConfigureAwait(false);

            return res?.Value != null;
        }

        /// <summary>
        /// Execute a filtered query (not supported by Memcached).
        /// </summary>
        public List<ConnectorEntity> Query(Func<ConnectorEntity, bool> filter)
        {
            throw new NotSupportedException("Memcached does not support server-side queries or key enumeration.");
        }

        /// <summary>
        /// Asynchronously execute a filtered query (not supported by Memcached).
        /// </summary>
        public Task<List<ConnectorEntity>> QueryAsync(Func<ConnectorEntity, bool> filter, CancellationToken ct = default)
        {
            throw new NotSupportedException("Memcached does not support server-side queries or key enumeration.");
        }
    }
}