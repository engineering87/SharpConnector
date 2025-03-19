// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using SharpConnector.Connectors.ArangoDb;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public override async Task<bool> DeleteAsync(string key)
        {
            return await _arangoDbWrapper.DeleteAsync(key);
        }

        /// <summary>
        /// Retrieves a object by key.
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
            if (connectorEntities != null)
                return connectorEntities.ToPayloadList<T>();
            return default;
        }

        /// <summary>
        /// Asynchronously retrieves all objects from the collection.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of objects.</returns>
        public override async Task<IEnumerable<T>> GetAllAsync()
        {
            var connectorEntities = await _arangoDbWrapper.GetAllAsync();
            return connectorEntities
                .Cast<T>()
                .ToList();
        }

        /// <summary>
        /// Asynchronously retrieves a object by key.
        /// </summary>
        /// <param name="key">The key of the object to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the object as a <see cref="T"/> object, or default if not found.</returns>
        public override async Task<T> GetAsync(string key)
        {
            var connectorEntity = await _arangoDbWrapper.GetAsync(key);
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
        /// Inserts a object with a specified key, value, and expiration time.
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
        /// Asynchronously inserts a object with a specified key and value.
        /// </summary>
        /// <param name="key">The key of the object to insert.</param>
        /// <param name="value">The value to insert into the object.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public override async Task<bool> InsertAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _arangoDbWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Asynchronously inserts an object with a specified key, value, and expiration time.
        /// </summary>
        /// <param name="key">The key of the object to insert.</param>
        /// <param name="value">The value to insert into the object.</param>
        /// <param name="expiration">The expiration time for the object.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public override async Task<bool> InsertAsync(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return await _arangoDbWrapper.InsertAsync(connectorEntity);
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
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public override async Task<bool> InsertManyAsync(IEnumerable<T> values)
        {
            var connectorEntityList = values
                .Cast<ConnectorEntity>()
                .ToList();
            return await _arangoDbWrapper.InsertManyAsync(connectorEntityList);
        }

        /// <summary>
        /// Updates a object with a specified key and value.
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
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public override async Task<bool> UpdateAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _arangoDbWrapper.UpdateAsync(connectorEntity);
        }
    }
}