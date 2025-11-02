// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharpConnector.Configuration;
using SharpConnector.Connectors.MongoDb;
using SharpConnector.Entities;
using SharpConnector.Utilities;

namespace SharpConnector.Operations
{
    public class MongoDbOperations<T> : Operations<T>
    {
        private readonly MongoDbWrapper _mongoDbWrapper;

        /// <summary>
        /// Create a new MongoDbOperations instance.
        /// </summary>
        /// <param name="mongoDbConfig">The MongoDB connector configuration.</param>
        public MongoDbOperations(MongoDbConfig mongoDbConfig)
        {
            _mongoDbWrapper = new MongoDbWrapper(mongoDbConfig);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override T Get(string key)
        {
            var connectorEntity = _mongoDbWrapper.Get(key);
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
            var connectorEntity = await _mongoDbWrapper.GetAsync(key, ct).ConfigureAwait(false);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Retrieve all values from MongoDB.
        /// </summary>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _mongoDbWrapper.GetAll();
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Asynchronously retrieve all values from MongoDB.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task whose result contains all values stored in MongoDB.</returns>
        public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            var connectorEntities = await _mongoDbWrapper.GetAllAsync(ct).ConfigureAwait(false);
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Set the key to hold the specified value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new MongoConnectorEntity(key, value, null);
            return _mongoDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the key to hold the specified value with expiration.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new MongoConnectorEntity(key, value, expiration);
            return _mongoDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Asynchronously set the key to hold the specified value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new MongoConnectorEntity(key, value, null);
            return await _mongoDbWrapper.InsertAsync(connectorEntity, ct).ConfigureAwait(false);
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
            var connectorEntity = new MongoConnectorEntity(key, value, expiration);
            return await _mongoDbWrapper.InsertAsync(connectorEntity, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert multiple values.
        /// </summary>
        /// <param name="values">The values to store.</param>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _mongoDbWrapper.InsertMany(values.ToMongoDbConnectorEntityList());
        }

        /// <summary>
        /// Insert multiple values with expiration.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration of the keys.</param>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _mongoDbWrapper.InsertMany(values.ToMongoDbConnectorEntityList(expiration));
        }

        /// <summary>
        /// Asynchronously insert multiple values.
        /// </summary>
        /// <param name="values">The values to store as an enumerable.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the insertion was successful; otherwise, false.</returns>
        public override async Task<bool> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default)
        {
            var list = values.Select(v => new MongoConnectorEntity(Guid.NewGuid().ToString(), v, null)).ToList();
            return await _mongoDbWrapper
                .InsertManyAsync(list, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override bool Delete(string key)
        {
            return _mongoDbWrapper.Delete(key);
        }

        /// <summary>
        /// Asynchronously remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            return await _mongoDbWrapper.DeleteAsync(key, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Update the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new MongoConnectorEntity(key, value, null);
            return _mongoDbWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Asynchronously update the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new MongoConnectorEntity(key, value, null);
            return await _mongoDbWrapper.UpdateAsync(connectorEntity, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public override bool Exists(string key)
        {
            return _mongoDbWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the item exists; otherwise, false.</returns>
        public override async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            return await _mongoDbWrapper.ExistsAsync(key, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a filtered query over the items in the database.
        /// </summary>
        /// <param name="filter">Predicate that selects items of type T.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of items of type T matching the filter.</returns>
        public override IEnumerable<T> Query(Func<T, bool> filter)
        {
            return _mongoDbWrapper
                .Query(me => filter(me.ToPayloadObject<T>()))
                .Select(me => me.ToPayloadObject<T>());
        }

        /// <summary>
        /// Asynchronously execute a filtered query over the items in the database.
        /// </summary>
        /// <param name="filter">Predicate that selects items of type T.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task whose result is an <see cref="IEnumerable{T}"/> of items of type T matching the filter.</returns>
        public override async Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter, CancellationToken ct = default)
        {
            var result = await _mongoDbWrapper
                .QueryAsync(me => filter(me.ToPayloadObject<T>()), ct)
                .ConfigureAwait(false);

            return result.Select(me => me.ToPayloadObject<T>());
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
            var entities = values.ToMongoDbConnectorEntityList();
            return await _mongoDbWrapper
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
            var entities = values.ToMongoDbConnectorEntityList(expiration);
            return await _mongoDbWrapper
                .InsertManyAsync(entities, ct)
                .ConfigureAwait(false);
        }
    }
}