// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using SharpConnector.Connectors.DynamoDb;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpConnector.Operations
{
    public class DynamoDbOperations<T> : Operations<T>
    {
        private readonly DynamoDbWrapper _dynamoDbWrapper;

        /// <summary>
        /// Create a new DynamoDbOperations instance.
        /// </summary>
        /// <param name="dynamoDbConfig">The DynamoDB connector config.</param>
        public DynamoDbOperations(DynamoDbConfig dynamoDbConfig)
        {
            _dynamoDbWrapper = new DynamoDbWrapper(dynamoDbConfig);
        }

        /// <summary>
        /// Get the value of a specific Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override T Get(string key)
        {
            var connectorEntity = _dynamoDbWrapper.Get(key);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Get the value of a specific Key asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override async Task<T> GetAsync(string key)
        {
            var connectorEntity = await _dynamoDbWrapper.GetAsync(key);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Get all values from the DynamoDB table.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _dynamoDbWrapper.GetAll();
            if (connectorEntities != null)
                return connectorEntities.ToPayloadList<T>();
            return default;
        }

        /// <summary>
        /// Asynchronously get all values from the DynamoDB table.
        /// </summary>
        /// <returns></returns>
        public override async Task<IEnumerable<T>> GetAllAsync()
        {
            var connectorEntities = await _dynamoDbWrapper.GetAllAsync();
            return connectorEntities?.ToPayloadList<T>() ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Insert a value into DynamoDB.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _dynamoDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Insert a value into DynamoDB with an expiration time.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <returns></returns>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _dynamoDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Insert a value into DynamoDB asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override async Task<bool> InsertAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _dynamoDbWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Insert a value into DynamoDB asynchronously with an expiration time.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <returns></returns>
        public override async Task<bool> InsertAsync(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return await _dynamoDbWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Insert multiple values into DynamoDB.
        /// </summary>
        /// <param name="values">A dictionary of key-value pairs to store.</param>
        /// <returns></returns>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _dynamoDbWrapper.InsertMany(values.ToConnectorEntityList());
        }

        /// <summary>
        /// Insert multiple items asynchronously.
        /// </summary>
        /// <param name="values">A collection of values to store.</param>
        /// <returns>A task representing the asynchronous operation, with the result being true if all insertions are successful.</returns>
        public override Task<bool> InsertManyAsync(IEnumerable<T> values)
        {
            var connectorEntityList = values
                .Cast<ConnectorEntity>()
                .ToList();
            return _dynamoDbWrapper.InsertManyAsync(connectorEntityList);
        }

        /// <summary>
        /// Insert multiple values into DynamoDB with an expiration time.
        /// </summary>
        /// <param name="values">A dictionary of key-value pairs to store.</param>
        /// <param name="expiration">The expiration time for the keys.</param>
        /// <returns></returns>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _dynamoDbWrapper.InsertMany(values.ToConnectorEntityList(expiration));
        }

        /// <summary>
        /// Delete a value from DynamoDB by key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override bool Delete(string key)
        {
            return _dynamoDbWrapper.Delete(key);
        }

        /// <summary>
        /// Delete a value from DynamoDB asynchronously by key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override async Task<bool> DeleteAsync(string key)
        {
            return await _dynamoDbWrapper.DeleteAsync(key);
        }

        /// <summary>
        /// Update a value in DynamoDB.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to update.</param>
        /// <returns></returns>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _dynamoDbWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Update a value in DynamoDB asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to update.</param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _dynamoDbWrapper.UpdateAsync(connectorEntity);
        }

        /// <summary>
        /// Checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public override bool Exists(string key)
        {
            return _dynamoDbWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>A task containing true if the item exists, false otherwise.</returns>
        public override async Task<bool> ExistsAsync(string key)
        {
            return await _dynamoDbWrapper.ExistsAsync(key);
        }
    }
}
