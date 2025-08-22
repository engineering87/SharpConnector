// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using SharpConnector.Connectors.RavenDb;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConnector.Operations
{
    public class RavenDbOperations<T> : Operations<T>
    {
        private readonly RavenDbWrapper _ravenDbWrapper;

        /// <summary>
        /// Create a new RavenDbOperations instance.
        /// </summary>
        /// <param name="ravenDbConfig">The RavenDB connector configuration.</param>
        public RavenDbOperations(RavenDbConfig ravenDbConfig)
        {
            _ravenDbWrapper = new RavenDbWrapper(ravenDbConfig);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override T Get(string key)
        {
            var connectorEntity = _ravenDbWrapper.Get(key);
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
            var connectorEntity = await _ravenDbWrapper
                .GetAsync(key, ct)
                .ConfigureAwait(false);

            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Retrieve all values from RavenDB.
        /// </summary>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _ravenDbWrapper.GetAll();
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Asynchronously retrieve all values from RavenDB.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task whose result contains all values stored in RavenDB.</returns>
        public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            var connectorEntities = await _ravenDbWrapper
                .GetAllAsync(ct)
                .ConfigureAwait(false);

            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Set the key to hold the specified value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _ravenDbWrapper.Insert(connectorEntity);
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
            return _ravenDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Asynchronously set the key to hold the specified value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _ravenDbWrapper
                .InsertAsync(connectorEntity, ct)
                .ConfigureAwait(false);
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
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return await _ravenDbWrapper
                .InsertAsync(connectorEntity, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Insert multiple values.
        /// </summary>
        /// <param name="values">The values to store.</param>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _ravenDbWrapper.InsertMany(values.ToConnectorEntityList());
        }

        /// <summary>
        /// Insert multiple values with expiration.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration of the keys.</param>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _ravenDbWrapper.InsertMany(values.ToConnectorEntityList(expiration));
        }

        /// <summary>
        /// Asynchronously insert multiple values.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if all insertions were successful; otherwise, false.</returns>
        public override async Task<bool> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default)
        {
            var list = values.Select(v => new ConnectorEntity(Guid.NewGuid().ToString(), v, null)).ToList();
            return await _ravenDbWrapper
                .InsertManyAsync(list, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override bool Delete(string key)
        {
            return _ravenDbWrapper.Delete(key);
        }

        /// <summary>
        /// Asynchronously remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            return await _ravenDbWrapper
                .DeleteAsync(key, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Update the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _ravenDbWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Asynchronously update the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _ravenDbWrapper
                .UpdateAsync(connectorEntity, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public override bool Exists(string key)
        {
            return _ravenDbWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the item exists; otherwise, false.</returns>
        public override async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            return await _ravenDbWrapper
                .ExistsAsync(key, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a filtered query over the items in the database.
        /// </summary>
        /// <param name="filter">A predicate that selects items of type T.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of items of type T matching the filter.</returns>
        public override IEnumerable<T> Query(Func<T, bool> filter)
        {
            return _ravenDbWrapper
                .Query(e => filter(e.ToPayloadObject<T>()))
                .Select(e => e.ToPayloadObject<T>());
        }

        /// <summary>
        /// Asynchronously execute a filtered query over the items in the database.
        /// </summary>
        /// <param name="filter">A predicate that selects items of type T.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task whose result is an <see cref="IEnumerable{T}"/> of items of type T matching the filter.</returns>
        public override async Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter, CancellationToken ct = default)
        {
            var result = await _ravenDbWrapper
                .QueryAsync(e => filter(e.ToPayloadObject<T>()), ct)
                .ConfigureAwait(false);

            return result.Select(e => e.ToPayloadObject<T>());
        }
    }
}