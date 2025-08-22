// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using SharpConnector.Connectors.DynamoDb;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConnector.Operations
{
    public class DynamoDbOperations<T> : Operations<T>
    {
        private readonly DynamoDbWrapper _dynamoDbWrapper;

        /// <summary>
        /// Create a new DynamoDbOperations instance.
        /// </summary>
        /// <param name="dynamoDbConfig">The DynamoDB connector configuration.</param>
        public DynamoDbOperations(DynamoDbConfig dynamoDbConfig)
        {
            _dynamoDbWrapper = new DynamoDbWrapper(dynamoDbConfig);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override T Get(string key)
        {
            var connectorEntity = _dynamoDbWrapper.Get(key);
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
            var connectorEntity = await _dynamoDbWrapper.GetAsync(key, ct).ConfigureAwait(false);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Retrieve all values from the DynamoDB table.
        /// </summary>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _dynamoDbWrapper.GetAll();
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Asynchronously retrieve all values from the DynamoDB table.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            var connectorEntities = await _dynamoDbWrapper.GetAllAsync(ct).ConfigureAwait(false);
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Insert a value into DynamoDB.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
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
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _dynamoDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Asynchronously insert a value into DynamoDB.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _dynamoDbWrapper.InsertAsync(connectorEntity, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously insert a value into DynamoDB with an expiration time.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertAsync(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return await _dynamoDbWrapper.InsertAsync(connectorEntity, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert multiple values into DynamoDB.
        /// </summary>
        /// <param name="values">A dictionary of key-value pairs to store.</param>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _dynamoDbWrapper.InsertMany(values.ToConnectorEntityList());
        }

        /// <summary>
        /// Asynchronously insert multiple values.
        /// </summary>
        /// <param name="values">A collection of values to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if all insertions succeeded; otherwise, false.</returns>
        public override Task<bool> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default)
        {
            var list = values.Select(v => new ConnectorEntity(Guid.NewGuid().ToString(), v, null)).ToList();
            return _dynamoDbWrapper.InsertManyAsync(list, ct);
        }

        /// <summary>
        /// Insert multiple values into DynamoDB with an expiration time.
        /// </summary>
        /// <param name="values">A dictionary of key-value pairs to store.</param>
        /// <param name="expiration">The expiration time for the keys.</param>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _dynamoDbWrapper.InsertMany(values.ToConnectorEntityList(expiration));
        }

        /// <summary>
        /// Delete a value from DynamoDB by key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override bool Delete(string key)
        {
            return _dynamoDbWrapper.Delete(key);
        }

        /// <summary>
        /// Asynchronously delete a value from DynamoDB by key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            return await _dynamoDbWrapper.DeleteAsync(key, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Update a value in DynamoDB.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to update.</param>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _dynamoDbWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Asynchronously update a value in DynamoDB.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to update.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _dynamoDbWrapper.UpdateAsync(connectorEntity, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public override bool Exists(string key)
        {
            return _dynamoDbWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the item exists; otherwise, false.</returns>
        public override async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            return await _dynamoDbWrapper.ExistsAsync(key, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a filtered query.
        /// </summary>
        public override IEnumerable<T> Query(Func<T, bool> filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            var result = _dynamoDbWrapper
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

            var result = await _dynamoDbWrapper
                .QueryAsync(e => filter(e.ToPayloadObject<T>()), ct)
                .ConfigureAwait(false);

            return result.Select(e => e.ToPayloadObject<T>());
        }
    }
}