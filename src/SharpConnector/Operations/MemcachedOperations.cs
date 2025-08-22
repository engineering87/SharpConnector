// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharpConnector.Configuration;
using SharpConnector.Connectors.Memcached;
using SharpConnector.Entities;
using SharpConnector.Utilities;

namespace SharpConnector.Operations
{
    public class MemcachedOperations<T> : Operations<T>
    {
        private readonly MemcachedWrapper _memcachedWrapper;

        /// <summary>
        /// Create a new MemcachedOperations instance.
        /// </summary>
        /// <param name="memcachedConfig">The Memcached connector configuration.</param>
        public MemcachedOperations(MemcachedConfig memcachedConfig)
        {
            _memcachedWrapper = new MemcachedWrapper(memcachedConfig);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override T Get(string key)
        {
            var connectorEntity = _memcachedWrapper.Get(key);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Asynchronously retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<T> GetAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var connectorEntity = await _memcachedWrapper.GetAsync(key, ct).ConfigureAwait(false);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Retrieve all values. (Not supported for Memcached)
        /// </summary>
        public override IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the key to hold the specified value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _memcachedWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the key to hold the specified value with expiration.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _memcachedWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Asynchronously set the key to hold the specified value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertAsync(string key, T value, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _memcachedWrapper.InsertAsync(connectorEntity).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously set the key to hold the specified value with expiration.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertAsync(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return await _memcachedWrapper.InsertAsync(connectorEntity, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert multiple values.
        /// </summary>
        /// <param name="values">The values to store.</param>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _memcachedWrapper.InsertMany(values.ToConnectorEntityList());
        }

        /// <summary>
        /// Insert multiple values with expiration.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration of the keys.</param>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _memcachedWrapper.InsertMany(values.ToConnectorEntityList(expiration));
        }

        /// <summary>
        /// Remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override bool Delete(string key)
        {
            return _memcachedWrapper.Delete(key);
        }

        /// <summary>
        /// Asynchronously remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await _memcachedWrapper.DeleteAsync(key, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Update the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _memcachedWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Asynchronously update the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _memcachedWrapper.UpdateAsync(connectorEntity, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve all objects. (Not supported by Memcached natively; wrapper returns whatever is implemented.)
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task whose result contains a list of objects.</returns>
        public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var connectorEntities = await _memcachedWrapper.GetAllAsync(ct).ConfigureAwait(false);
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Asynchronously insert multiple values.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if all insertions were successful; otherwise, false.</returns>
        public override async Task<bool> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var list = values.Select(v => new ConnectorEntity(Guid.NewGuid().ToString(), v, null)).ToList();
            return await _memcachedWrapper
                .InsertManyAsync(list, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public override bool Exists(string key)
        {
            return _memcachedWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the item exists; otherwise, false.</returns>
        public override async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await _memcachedWrapper.ExistsAsync(key, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a filtered query. (Not supported for Memcached)
        /// </summary>
        public override IEnumerable<T> Query(Func<T, bool> filter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously execute a filtered query. (Not supported for Memcached)
        /// </summary>
        /// <param name="filter">Predicate that selects items of type T.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}