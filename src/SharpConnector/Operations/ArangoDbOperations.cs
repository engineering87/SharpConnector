// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using SharpConnector.Connectors.ArangoDb;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConnector.Operations
{
    /// <summary>
    /// Provides operations for interacting with an ArangoDB collection.
    /// </summary>
    /// <typeparam name="T">The type of the payload stored in the collection.</typeparam>
    public class ArangoDbOperations<T> : Operations<T>
    {
        private readonly ArangoDbWrapper _arangoDbWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArangoDbOperations{T}"/> class.
        /// </summary>
        /// <param name="arangoDbConfig">The ArangoDB connector configuration.</param>
        public ArangoDbOperations(ArangoDbConfig arangoDbConfig)
        {
            _arangoDbWrapper = new ArangoDbWrapper(arangoDbConfig);
        }

        /// <summary>
        /// Deletes a document by key.
        /// </summary>
        /// <param name="key">The key of the object to delete.</param>
        /// <returns>True if the document was deleted successfully.</returns>
        public override bool Delete(string key)
        {
            return _arangoDbWrapper.Delete(key);
        }

        /// <summary>
        /// Asynchronously deletes an object by key.
        /// </summary>
        /// <param name="key">The key of the object to delete.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public override Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            return _arangoDbWrapper.DeleteAsync(key, ct);
        }

        /// <summary>
        /// Retrieves an object by key.
        /// </summary>
        /// <param name="key">The key of the object to retrieve.</param>
        /// <returns>The retrieved document as a <see cref="T"/> object, or default if not found.</returns>
        public override T Get(string key)
        {
            var connectorEntity = _arangoDbWrapper.Get(key);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Retrieves all objects from the collection.
        /// </summary>
        /// <returns>An enumerable collection of objects as <see cref="T"/> objects.</returns>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _arangoDbWrapper.GetAll();
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Asynchronously retrieves all objects from the collection.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of objects.</returns>
        public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            var connectorEntities = await _arangoDbWrapper.GetAllAsync(ct).ConfigureAwait(false);
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Asynchronously retrieves an object by key.
        /// </summary>
        /// <param name="key">The key of the object to retrieve.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the object as a <see cref="T"/> object, or default if not found.</returns>
        public override async Task<T> GetAsync(string key, CancellationToken ct = default)
        {
            var connectorEntity = await _arangoDbWrapper.GetAsync(key, ct).ConfigureAwait(false);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Inserts an object with a specified key and value.
        /// </summary>
        /// <param name="key">The key of the object to insert.</param>
        /// <param name="value">The value to insert into the object.</param>
        /// <returns>True if the insertion was successful.</returns>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value);
            return _arangoDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Inserts an object with a specified key, value, and expiration time.
        /// </summary>
        /// <param name="key">The key of the object to insert.</param>
        /// <param name="value">The value to insert into the object.</param>
        /// <param name="expiration">The expiration time for the object.</param>
        /// <returns>True if the insertion was successful.</returns>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _arangoDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Asynchronously inserts an object with a specified key and value.
        /// </summary>
        /// <param name="key">The key of the object to insert.</param>
        /// <param name="value">The value to insert into the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public override Task<bool> InsertAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _arangoDbWrapper.InsertAsync(connectorEntity, ct);
        }

        /// <summary>
        /// Asynchronously inserts an object with a specified key, value, and expiration time.
        /// </summary>
        /// <param name="key">The key of the object to insert.</param>
        /// <param name="value">The value to insert into the object.</param>
        /// <param name="expiration">The expiration time for the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public override Task<bool> InsertAsync(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _arangoDbWrapper.InsertAsync(connectorEntity, ct);
        }

        /// <summary>
        /// Inserts multiple objects.
        /// </summary>
        /// <param name="values">A dictionary containing key-value pairs of objects to insert.</param>
        /// <returns>True if all objects were inserted successfully.</returns>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _arangoDbWrapper.InsertMany(values.ToConnectorEntityList());
        }

        /// <summary>
        /// Inserts multiple objects with a specified expiration time.
        /// </summary>
        /// <param name="values">A dictionary containing key-value pairs of objects to insert.</param>
        /// <param name="expiration">The expiration time for the objects.</param>
        /// <returns>True if all objects were inserted successfully.</returns>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _arangoDbWrapper.InsertMany(values.ToConnectorEntityList(expiration));
        }

        /// <summary>
        /// Asynchronously inserts multiple objects.
        /// </summary>
        /// <param name="values">A collection of objects to insert.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public override Task<bool> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default)
        {
            var list = values.Select(v => new ConnectorEntity(Guid.NewGuid().ToString(), v, null)).ToList();
            return _arangoDbWrapper.InsertManyAsync(list, ct);
        }

        /// <summary>
        /// Updates an object with a specified key and value.
        /// </summary>
        /// <param name="key">The key of the object to update.</param>
        /// <param name="value">The new value to update in the object.</param>
        /// <returns>True if the update was successful.</returns>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _arangoDbWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Asynchronously updates an object with a specified key and value.
        /// </summary>
        /// <param name="key">The key of the object to update.</param>
        /// <param name="value">The new value to update in the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public override Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _arangoDbWrapper.UpdateAsync(connectorEntity, ct);
        }

        /// <summary>
        /// Checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public override bool Exists(string key)
        {
            return _arangoDbWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task containing true if the item exists, false otherwise.</returns>
        public override Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            return _arangoDbWrapper.ExistsAsync(key, ct);
        }

        /// <summary>
        /// Executes a filtered query over items in the collection.
        /// </summary>
        /// <param name="filter">Predicate that selects items of type T.</param>
        public override IEnumerable<T> Query(Func<T, bool> filter)
        {
            return _arangoDbWrapper
                .Query(e => filter(e.ToPayloadObject<T>()))
                .Select(e => e.ToPayloadObject<T>());
        }

        /// <summary>
        /// Asynchronously executes a filtered query over items in the collection.
        /// </summary>
        /// <param name="filter">Predicate that selects items of type T.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter, CancellationToken ct = default)
        {
            var result = await _arangoDbWrapper
                .QueryAsync(e => filter(e.ToPayloadObject<T>()), ct)
                .ConfigureAwait(false);

            return result.Select(e => e.ToPayloadObject<T>());
        }
    }
}