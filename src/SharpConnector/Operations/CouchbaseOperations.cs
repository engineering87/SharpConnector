// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharpConnector.Configuration;
using SharpConnector.Connectors.Couchbase;
using SharpConnector.Entities;
using SharpConnector.Utilities;

namespace SharpConnector.Operations
{
    public class CouchbaseOperations<T> : Operations<T>
    {
        private readonly CouchbaseWrapper _couchbaseWrapper;

        /// <summary>
        /// Create a new CouchbaseOperations instance.
        /// </summary>
        /// <param name="couchbaseConfig">The Couchbase connector configuration.</param>
        public CouchbaseOperations(CouchbaseConfig couchbaseConfig)
        {
            _couchbaseWrapper = new CouchbaseWrapper(couchbaseConfig);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// (Uses async wrapper under the hood.)
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override T Get(string key)
        {
            var connectorEntity = _couchbaseWrapper.GetAsync(key, CancellationToken.None)
                                                   .GetAwaiter()
                                                   .GetResult();
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
            var connectorEntity = await _couchbaseWrapper.GetAsync(key, ct).ConfigureAwait(false);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Retrieve all values from the Couchbase bucket.
        /// (Uses async wrapper under the hood.)
        /// </summary>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _couchbaseWrapper.GetAllAsync(CancellationToken.None)
                                                     .GetAwaiter()
                                                     .GetResult();
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Asynchronously retrieve all values from the Couchbase bucket.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            var connectorEntities = await _couchbaseWrapper.GetAllAsync(ct).ConfigureAwait(false);
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Insert a new object into the Couchbase bucket.
        /// (Uses async wrapper under the hood.)
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _couchbaseWrapper
                .InsertAsync(connectorEntity, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Insert a new object with an expiration time into the Couchbase bucket.
        /// (Uses async wrapper under the hood.)
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _couchbaseWrapper
                .InsertAsync(connectorEntity, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Asynchronously insert a new object into the Couchbase bucket.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override Task<bool> InsertAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _couchbaseWrapper.InsertAsync(connectorEntity, ct);
        }

        /// <summary>
        /// Asynchronously insert a new object with an expiration time into the Couchbase bucket.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override Task<bool> InsertAsync(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _couchbaseWrapper.InsertAsync(connectorEntity, ct);
        }

        /// <summary>
        /// Insert multiple objects into the Couchbase bucket.
        /// (Uses async wrapper under the hood.)
        /// </summary>
        /// <param name="values">The values to store.</param>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            var entities = values.ToConnectorEntityList();
            return _couchbaseWrapper.InsertManyAsync(entities, CancellationToken.None)
                                    .GetAwaiter()
                                    .GetResult();
        }

        /// <summary>
        /// Insert multiple objects with expiration times into the Couchbase bucket.
        /// (Uses async wrapper under the hood.)
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration time for the keys.</param>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            var entities = values.ToConnectorEntityList(expiration);
            return _couchbaseWrapper.InsertManyAsync(entities, CancellationToken.None)
                                    .GetAwaiter()
                                    .GetResult();
        }

        /// <summary>
        /// Asynchronously insert multiple values into the Couchbase bucket.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if all insertions succeeded; otherwise, false.</returns>
        public override async Task<bool> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default)
        {
            var entities = values.Select(value =>
            {
                ct.ThrowIfCancellationRequested();
                string key = Guid.NewGuid().ToString();
                return new ConnectorEntity(key, value, null);
            }).ToList();

            return await _couchbaseWrapper.InsertManyAsync(entities, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete an object by key from the Couchbase bucket.
        /// (Uses async wrapper under the hood.)
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override bool Delete(string key)
        {
            return _couchbaseWrapper.DeleteAsync(key, CancellationToken.None)
                                    .GetAwaiter()
                                    .GetResult();
        }

        /// <summary>
        /// Asynchronously delete an object by key from the Couchbase bucket.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            return _couchbaseWrapper.DeleteAsync(key, ct);
        }

        /// <summary>
        /// Update an existing object in the Couchbase bucket.
        /// (Uses async wrapper under the hood.)
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to update.</param>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _couchbaseWrapper.UpdateAsync(connectorEntity, CancellationToken.None)
                                    .GetAwaiter()
                                    .GetResult();
        }

        /// <summary>
        /// Asynchronously update an existing object in the Couchbase bucket.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to update.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _couchbaseWrapper.UpdateAsync(connectorEntity, ct);
        }

        /// <summary>
        /// Check whether an item exists by its key.
        /// (Uses async wrapper under the hood.)
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public override bool Exists(string key)
        {
            return _couchbaseWrapper.ExistsAsync(key, CancellationToken.None)
                                    .GetAwaiter()
                                    .GetResult();
        }

        /// <summary>
        /// Asynchronously check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the item exists; otherwise, false.</returns>
        public override Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            return _couchbaseWrapper.ExistsAsync(key, ct);
        }

        /// <summary>
        /// Execute a filtered query.
        /// </summary>
        public override IEnumerable<T> Query(Func<T, bool> filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            var result = _couchbaseWrapper
                .Query(e => filter(e.ToPayloadObject<T>()));

            return result.Select(e => e.ToPayloadObject<T>());
        }

        /// <summary>
        /// Asynchronously execute a filtered query.
        /// </summary>
        /// <param name="filter">Predicate that selects items of type T.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            ct.ThrowIfCancellationRequested();

            var result = await _couchbaseWrapper
                .QueryAsync(e => filter(e.ToPayloadObject<T>()), ct)
                .ConfigureAwait(false);

            return result.Select(e => e.ToPayloadObject<T>());
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
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
        public override async Task<bool> InsertManyAsync(Dictionary<string, T> values, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var entities = values.ToConnectorEntityList();
            return await _couchbaseWrapper
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
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
        public override async Task<bool> InsertManyAsync(Dictionary<string, T> values, TimeSpan expiration, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var entities = values.ToConnectorEntityList(expiration);
            return await _couchbaseWrapper
                .InsertManyAsync(entities, ct)
                .ConfigureAwait(false);
        }
    }
}