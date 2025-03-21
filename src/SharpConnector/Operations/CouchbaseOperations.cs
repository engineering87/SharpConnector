// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="couchbaseConfig">The Couchbase connector config.</param>
        public CouchbaseOperations(CouchbaseConfig couchbaseConfig)
        {
            _couchbaseWrapper = new CouchbaseWrapper(couchbaseConfig);
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override T Get(string key)
        {
            var connectorEntity = _couchbaseWrapper.GetAsync(key).Result;
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Get the value of Key asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override async Task<T> GetAsync(string key)
        {
            var connectorEntity = await _couchbaseWrapper.GetAsync(key);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Get all values from Couchbase bucket.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _couchbaseWrapper.GetAllAsync().Result;
            return connectorEntities?.ToPayloadList<T>();
        }

        /// <summary>
        /// Insert a new object into the Couchbase bucket.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _couchbaseWrapper.InsertAsync(connectorEntity).Result;
        }

        /// <summary>
        /// Insert a new object with an expiration time into the Couchbase bucket.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <returns></returns>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _couchbaseWrapper.InsertAsync(connectorEntity).Result;
        }

        /// <summary>
        /// Insert a new object into the Couchbase bucket asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override async Task<bool> InsertAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _couchbaseWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Insert a new object with an expiration time into the Couchbase bucket asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <returns></returns>
        public override async Task<bool> InsertAsync(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return await _couchbaseWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Insert multiple objects into the Couchbase bucket.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <returns></returns>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            var entities = values.ToConnectorEntityList();
            return _couchbaseWrapper.InsertManyAsync(entities).Result;
        }

        /// <summary>
        /// Insert multiple objects with expiration times into the Couchbase bucket.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration time for the keys.</param>
        /// <returns></returns>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            var entities = values.ToConnectorEntityList(expiration);
            return _couchbaseWrapper.InsertManyAsync(entities).Result;
        }

        /// <summary>
        /// Deletes an object by key from the Couchbase bucket.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override bool Delete(string key)
        {
            return _couchbaseWrapper.DeleteAsync(key).Result;
        }

        /// <summary>
        /// Deletes an object by key from the Couchbase bucket asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override async Task<bool> DeleteAsync(string key)
        {
            return await _couchbaseWrapper.DeleteAsync(key);
        }

        /// <summary>
        /// Updates an existing object in the Couchbase bucket.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to update.</param>
        /// <returns></returns>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _couchbaseWrapper.UpdateAsync(connectorEntity).Result;
        }

        /// <summary>
        /// Updates an existing object in the Couchbase bucket asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to update.</param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _couchbaseWrapper.UpdateAsync(connectorEntity);
        }

        /// <summary>
        /// Get all values from Couchbase bucket asynchronously.
        /// </summary>
        /// <returns>A task representing a list of all values in the bucket.</returns>
        public override async Task<IEnumerable<T>> GetAllAsync()
        {
            var connectorEntities = await _couchbaseWrapper.GetAllAsync();
            return connectorEntities?.ToPayloadList<T>();
        }

        /// <summary>
        /// Insert multiple values into the Couchbase bucket asynchronously.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <returns>A task representing whether all insertions succeeded.</returns>
        public override async Task<bool> InsertManyAsync(IEnumerable<T> values)
        {
            var entities = values.Select(value =>
            {
                string key = Guid.NewGuid().ToString();
                return new ConnectorEntity(key, value, null);
            }).ToList();

            return await _couchbaseWrapper.InsertManyAsync(entities);
        }

        /// <summary>
        /// Checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public override bool Exists(string key)
        {
            return _couchbaseWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>A task containing true if the item exists, false otherwise.</returns>
        public override async Task<bool> ExistsAsync(string key)
        {
            return await _couchbaseWrapper.ExistsAsync(key);
        }
    }
}
