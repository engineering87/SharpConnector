// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Connectors.LiteDb;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using System.Linq;

namespace SharpConnector.Operations
{
    public class LiteDbOperations<T> : Operations<T>
    {
        private readonly LiteDbWrapper _liteDbWrapper;

        /// <summary>
        /// Create a new LiteDbOperations instance.
        /// </summary>
        /// <param name="liteDbConfig">The LiteDB connector configuration.</param>
        public LiteDbOperations(LiteDbConfig liteDbConfig)
        {
            _liteDbWrapper = new LiteDbWrapper(liteDbConfig);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override T Get(string key)
        {
            var connectorEntity = _liteDbWrapper.Get(key);
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
            var connectorEntity = await _liteDbWrapper.GetAsync(key, ct).ConfigureAwait(false);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Retrieve all values from LiteDB.
        /// </summary>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _liteDbWrapper.GetAll();
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Set the key to hold the specified value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new LiteDbConnectorEntity(key, value, null);
            return _liteDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the key to hold the specified value with expiration.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new LiteDbConnectorEntity(key, value, expiration);
            return _liteDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Asynchronously set the key to hold the specified value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new LiteDbConnectorEntity(key, value, null);
            return await _liteDbWrapper.InsertAsync(connectorEntity, ct).ConfigureAwait(false);
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
            var connectorEntity = new LiteDbConnectorEntity(key, value, expiration);
            return await _liteDbWrapper.InsertAsync(connectorEntity, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert multiple values.
        /// </summary>
        /// <param name="values">The values to store.</param>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _liteDbWrapper.InsertMany(values.ToLiteDbConnectorEntityList());
        }

        /// <summary>
        /// Insert multiple values with expiration.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration of the keys.</param>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _liteDbWrapper.InsertMany(values.ToLiteDbConnectorEntityList(expiration));
        }

        /// <summary>
        /// Remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override bool Delete(string key)
        {
            return _liteDbWrapper.Delete(key);
        }

        /// <summary>
        /// Asynchronously remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            return await _liteDbWrapper.DeleteAsync(key, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Update the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new LiteDbConnectorEntity(key, value, null);
            return _liteDbWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Asynchronously update the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new LiteDbConnectorEntity(key, value, null);
            return await _liteDbWrapper.UpdateAsync(connectorEntity, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieve all values.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task whose result contains all stored objects.</returns>
        public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            var connectorEntities = await _liteDbWrapper.GetAllAsync(ct).ConfigureAwait(false);
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Asynchronously insert multiple values.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the insertion succeeded; otherwise, false.</returns>
        public override async Task<bool> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default)
        {
            var list = values.Select(v => new LiteDbConnectorEntity(Guid.NewGuid().ToString(), v, null)).ToList();
            return await _liteDbWrapper
                .InsertManyAsync(list, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public override bool Exists(string key)
        {
            return _liteDbWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the item exists; otherwise, false.</returns>
        public override async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            return await _liteDbWrapper.ExistsAsync(key, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a filtered query.
        /// </summary>
        public override IEnumerable<T> Query(Func<T, bool> filter)
        {
            return _liteDbWrapper
                .Query(le => filter(le.ToPayloadObject<T>()))
                .Select(le => le.ToPayloadObject<T>());
        }

        /// <summary>
        /// Asynchronously execute a filtered query. (Not supported in this operations class)
        /// </summary>
        /// <param name="filter">Predicate that selects items of type T.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter, CancellationToken ct = default)
        {
            var result = await _liteDbWrapper
                .QueryAsync(le => filter(le.ToPayloadObject<T>()), ct)
                .ConfigureAwait(false);

            return result.Select(le => le.ToPayloadObject<T>());
        }

        /// <summary>
        /// Inserts multiple key–value pairs asynchronously.
        /// </summary>
        /// <param name="values">
        /// A dictionary containing the key/payload pairs to insert. Cannot be null.
        /// </param>
        /// <param name="ct">
        /// A token to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is
        /// <c>true</c> if all items were inserted successfully; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
        public override async Task<bool> InsertManyAsync(Dictionary<string, T> values, CancellationToken ct = default)
        {
            var entities = values.ToLiteDbConnectorEntityList();
            return await _liteDbWrapper
                .InsertManyAsync(entities, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Inserts multiple key–value pairs with a common expiration asynchronously.
        /// </summary>
        /// <param name="values">
        /// A dictionary containing the key/payload pairs to insert. Cannot be null.
        /// </param>
        /// <param name="expiration">
        /// The time-to-live to apply to each inserted key.
        /// </param>
        /// <param name="ct">
        /// A token to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is
        /// <c>true</c> if all items were inserted successfully; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
        public override async Task<bool> InsertManyAsync(Dictionary<string, T> values, TimeSpan expiration, CancellationToken ct = default)
        {
            var entities = values.ToLiteDbConnectorEntityList(expiration);
            return await _liteDbWrapper
                .InsertManyAsync(entities, ct)
                .ConfigureAwait(false);
        }
    }
}