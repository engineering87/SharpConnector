﻿// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpConnector.Configuration;
using SharpConnector.Connectors.MongoDb;
using SharpConnector.Connectors.RavenDb;
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
        /// <param name="mongoDbConfig">The MongoDb connector config.</param>
        public MongoDbOperations(MongoDbConfig mongoDbConfig)
        {
            _mongoDbWrapper = new MongoDbWrapper(mongoDbConfig);
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override T Get(string key)
        {
            var connectorEntity =_mongoDbWrapper.Get(key);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override async Task<T> GetAsync(string key)
        {
            var connectorEntity = await _mongoDbWrapper.GetAsync(key);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Get all values from MongoDb.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _mongoDbWrapper.GetAll();
            if (connectorEntities != null)
                return connectorEntities.ToPayloadList<T>();
            return default;
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new MongoConnectorEntity(key, value, null);
            return _mongoDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <returns></returns>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new MongoConnectorEntity(key, value, expiration);
            return _mongoDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override async Task<bool> InsertAsync(string key, T value)
        {
            var connectorEntity = new MongoConnectorEntity(key, value, null);
            return await _mongoDbWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <returns></returns>
        public override async Task<bool> InsertAsync(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new MongoConnectorEntity(key, value, expiration);
            return await _mongoDbWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <returns></returns>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _mongoDbWrapper.InsertMany(values.ToMongoDbConnectorEntityList());
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration of the keys.</param>
        /// <returns></returns>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _mongoDbWrapper.InsertMany(values.ToMongoDbConnectorEntityList(expiration));
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override bool Delete(string key)
        {
            return _mongoDbWrapper.Delete(key);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override async Task<bool> DeleteAsync(string key)
        {
            return await _mongoDbWrapper.DeleteAsync(key);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new MongoConnectorEntity(key, value, null);
            return _mongoDbWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(string key, T value)
        {
            var connectorEntity = new MongoConnectorEntity(key, value, null);
            return await _mongoDbWrapper.UpdateAsync(connectorEntity);
        }

        /// <summary>
        /// Get all values asynchronously from MongoDb.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, which wraps an enumerable of all objects.</returns>
        public override async Task<IEnumerable<T>> GetAllAsync()
        {
            var connectorEntities = await _mongoDbWrapper.GetAllAsync();
            return connectorEntities
                .Cast<T>()
                .ToList();
        }

        /// <summary>
        /// Insert multiple values asynchronously.
        /// </summary>
        /// <param name="values">The values to store as an enumerable.</param>
        /// <returns>A task representing the asynchronous operation, which returns true if the insertion was successful.</returns>
        public override async Task<bool> InsertManyAsync(IEnumerable<T> values)
        {
            var connectorEntityList = values
                .Cast<MongoConnectorEntity>()
                .ToList();
            return await _mongoDbWrapper.InsertManyAsync(connectorEntityList);
        }

        /// <summary>
        /// Checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public override bool Exists(string key)
        {
            return _mongoDbWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>A task containing true if the item exists, false otherwise.</returns>
        public override async Task<bool> ExistsAsync(string key)
        {
            return await _mongoDbWrapper.ExistsAsync(key);
        }

        /// <summary>
        /// Esegue una query filtrata sugli elementi nel database.
        /// </summary>
        /// <param name="filter">Funzione di filtro che seleziona gli elementi di tipo T.</param>
        /// <returns>Un IEnumerable di elementi di tipo T che soddisfano il filtro.</returns>
        public override IEnumerable<T> Query(Func<T, bool> filter)
        {
            bool castedFilter(MongoConnectorEntity entity) =>
                entity is T tItem && filter(tItem);

            return _mongoDbWrapper
                .Query(castedFilter)
                .Cast<T>();
        }

        /// <summary>
        /// Esegue una query asincrona filtrata sugli elementi nel database.
        /// </summary>
        /// <param name="filter">Funzione di filtro che seleziona gli elementi di tipo T.</param>
        /// <returns>Un Task contenente un IEnumerable di elementi di tipo T che soddisfano il filtro.</returns>
        public override async Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter)
        {
            bool castedFilter(MongoConnectorEntity entity) =>
            entity is T tItem && filter(tItem);

            var result = await _mongoDbWrapper.QueryAsync(castedFilter);
            return result.Cast<T>();
        }
    }
}
