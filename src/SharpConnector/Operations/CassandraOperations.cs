// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using SharpConnector.Connectors.Cassandra;
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
    /// Provides operations for interacting with an Apache Cassandra table.
    /// </summary>
    /// <typeparam name="T">The type of the payload stored in the table.</typeparam>
    public class CassandraOperations<T> : Operations<T>
    {
        private readonly CassandraWrapper _cassandraWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CassandraOperations{T}"/> class.
        /// </summary>
        /// <param name="cassandraConfig">The Cassandra connector configuration.</param>
        public CassandraOperations(CassandraConfig cassandraConfig)
        {
            _cassandraWrapper = new CassandraWrapper(cassandraConfig);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override T Get(string key)
        {
            var connectorEntity = _cassandraWrapper.Get(key);
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
            var connectorEntity = await _cassandraWrapper.GetAsync(key, ct).ConfigureAwait(false);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Retrieve all values.
        /// </summary>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _cassandraWrapper.GetAll();
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Asynchronously retrieve all values.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            var connectorEntities = await _cassandraWrapper.GetAllAsync(ct).ConfigureAwait(false);
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Insert (upsert) the specified key and value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _cassandraWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Insert (upsert) the specified key and value with expiration.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration time for the value.</param>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _cassandraWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Asynchronously insert (upsert) the specified key and value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override Task<bool> InsertAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _cassandraWrapper.InsertAsync(connectorEntity, ct);
        }

        /// <summary>
        /// Asynchronously insert (upsert) the specified key and value with expiration.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration time for the value.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override Task<bool> InsertAsync(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _cassandraWrapper.InsertAsync(connectorEntity, ct);
        }

        /// <summary>
        /// Insert multiple values.
        /// </summary>
        /// <param name="values">The values to store.</param>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _cassandraWrapper.InsertMany(values.ToConnectorEntityList());
        }

        /// <summary>
        /// Insert multiple values with expiration.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration time for the values.</param>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _cassandraWrapper.InsertMany(values.ToConnectorEntityList(expiration));
        }

        /// <summary>
        /// Asynchronously insert multiple values.
        /// </summary>
        /// <param name="values">A collection of objects to insert.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<IReadOnlyCollection<string>> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(values);
            var list = values.Select(v => new ConnectorEntity(Guid.NewGuid().ToString(), v, null)).ToList();
            var success = await _cassandraWrapper.InsertManyAsync(list, ct).ConfigureAwait(false);
            return success
                ? list.Select(e => e.Key).ToList().AsReadOnly()
                : (IReadOnlyCollection<string>)Array.Empty<string>();
        }

        /// <summary>
        /// Asynchronously insert multiple key/value pairs.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertManyAsync(Dictionary<string, T> values, CancellationToken ct = default)
        {
            var entities = values.ToConnectorEntityList();
            return await _cassandraWrapper
                .InsertManyAsync(entities, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously insert multiple key/value pairs with a common expiration.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration time for the values.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertManyAsync(Dictionary<string, T> values, TimeSpan expiration, CancellationToken ct = default)
        {
            var entities = values.ToConnectorEntityList(expiration);
            return await _cassandraWrapper
                .InsertManyAsync(entities, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override bool Delete(string key)
        {
            return _cassandraWrapper.Delete(key);
        }

        /// <summary>
        /// Asynchronously remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            return _cassandraWrapper.DeleteAsync(key, ct);
        }

        /// <summary>
        /// Update the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _cassandraWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Asynchronously update the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _cassandraWrapper.UpdateAsync(connectorEntity, ct);
        }

        /// <summary>
        /// Check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public override bool Exists(string key)
        {
            return _cassandraWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            return _cassandraWrapper.ExistsAsync(key, ct);
        }

        /// <summary>
        /// Execute a filtered query over the items in the table.
        /// </summary>
        /// <param name="filter">Predicate that selects items of type T.</param>
        public override IEnumerable<T> Query(Func<T, bool> filter)
        {
            return _cassandraWrapper
                .Query(e => filter(e.ToPayloadObject<T>()))
                .Select(e => e.ToPayloadObject<T>());
        }

        /// <summary>
        /// Asynchronously execute a filtered query over the items in the table.
        /// </summary>
        /// <param name="filter">Predicate that selects items of type T.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter, CancellationToken ct = default)
        {
            var result = await _cassandraWrapper
                .QueryAsync(e => filter(e.ToPayloadObject<T>()), ct)
                .ConfigureAwait(false);

            return result.Select(e => e.ToPayloadObject<T>());
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _cassandraWrapper?.Dispose();
            base.Dispose();
        }
    }
}
