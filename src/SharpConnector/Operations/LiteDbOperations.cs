// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Connectors.LiteDb;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using SharpConnector.Connectors.Redis;
using System.Linq;
using SharpConnector.Connectors.Memcached;

namespace SharpConnector.Operations
{
    public class LiteDbOperations<T> : Operations<T>
    {
        private readonly LiteDbWrapper _liteDbWrapper;

        /// <summary>
        /// Create a new LiteDbOperations instance.
        /// </summary>
        /// <param name="liteDbConfig">The LiteDb connector config.</param>
        public LiteDbOperations(LiteDbConfig liteDbConfig)
        {
            _liteDbWrapper = new LiteDbWrapper(liteDbConfig);
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override T Get(string key)
        {
            var connectorEntity = _liteDbWrapper.Get(key);
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
            var connectorEntity = await _liteDbWrapper.GetAsync(key);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Get all values from LiteDb.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _liteDbWrapper.GetAll();
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
            var connectorEntity = new LiteDbConnectorEntity(key, value, null);
            return _liteDbWrapper.Insert(connectorEntity);
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
            var connectorEntity = new LiteDbConnectorEntity(key, value, expiration);
            return _liteDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override async Task<bool> InsertAsync(string key, T value)
        {
            var connectorEntity = new LiteDbConnectorEntity(key, value, null);
            return await _liteDbWrapper.InsertAsync(connectorEntity);
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
            var connectorEntity = new LiteDbConnectorEntity(key, value, expiration);
            return await _liteDbWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <returns></returns>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _liteDbWrapper.InsertMany(values.ToLiteDbConnectorEntityList());
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration of the keys.</param>
        /// <returns></returns>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _liteDbWrapper.InsertMany(values.ToLiteDbConnectorEntityList(expiration));
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override bool Delete(string key)
        {
            return _liteDbWrapper.Delete(key);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override async Task<bool> DeleteAsync(string key)
        {
            return await _liteDbWrapper.DeleteAsync(key);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new LiteDbConnectorEntity(key, value, null);
            return _liteDbWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(string key, T value)
        {
            var connectorEntity = new LiteDbConnectorEntity(key, value, null);
            return await _liteDbWrapper.UpdateAsync(connectorEntity);
        }

        public override async Task<IEnumerable<T>> GetAllAsync()
        {
            var connectorEntities = await _liteDbWrapper.GetAllAsync();
            return connectorEntities
                .Cast<T>()
                .ToList();
        }

        public override async Task<bool> InsertManyAsync(IEnumerable<T> values)
        {
            var connectorEntityList = values
                .Cast<LiteDbConnectorEntity>()
                .ToList();
            return await _liteDbWrapper.InsertManyAsync(connectorEntityList);
        }
    }
}
