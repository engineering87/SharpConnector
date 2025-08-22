// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharpConnector.Configuration;
using SharpConnector.Connectors.Redis;
using SharpConnector.Entities;
using SharpConnector.Utilities;

namespace SharpConnector.Operations
{
    public class RedisOperations<T> : Operations<T>
    {
        private readonly RedisWrapper _redisWrapper;

        /// <summary>
        /// Create a new RedisOperations instance.
        /// </summary>
        /// <param name="redisConfig">The Redis connector config.</param>
        public RedisOperations(RedisConfig redisConfig)
        {
            _redisWrapper = new RedisWrapper(redisConfig);
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override T Get(string key)
        {
            var connectorEntity = _redisWrapper.Get(key);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Get the value of Key asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<T> GetAsync(string key, CancellationToken ct = default)
        {
            var connectorEntity = await _redisWrapper.GetAsync(key, 0, ct).ConfigureAwait(false);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Get all values from Redis database.
        /// </summary>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _redisWrapper.GetAll();
            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Insert(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _redisWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _redisWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the Key to hold the value asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _redisWrapper
                .InsertAsync(connectorEntity, 0, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Set the Key to hold the value asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertAsync(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return await _redisWrapper
                .InsertAsync(connectorEntity, 0, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="values">The values to store.</param>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _redisWrapper.InsertMany(values.ToConnectorEntityList());
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration of the keys.</param>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _redisWrapper.InsertMany(values.ToConnectorEntityList(expiration));
        }

        /// <summary>
        /// Insert multiple key-value pairs into Redis asynchronously.
        /// </summary>
        /// <param name="values">A collection of values to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default)
        {
            var list = values.Select(v => new ConnectorEntity(Guid.NewGuid().ToString(), v, null)).ToList();
            return await _redisWrapper
                .InsertManyAsync(list, 0, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public override bool Delete(string key)
        {
            return _redisWrapper.Delete(key);
        }

        /// <summary>
        /// Removes the specified Key asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            return await _redisWrapper
                .DeleteAsync(key, 0, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _redisWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Updates the specified Key asynchronously.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _redisWrapper
                .UpdateAsync(connectorEntity, 0, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get all values from Redis asynchronously.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with the result being all values stored in Redis.</returns>
        public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            var connectorEntities = await _redisWrapper
                .GetAllAsync(0, ct)
                .ConfigureAwait(false);

            return connectorEntities?.ToPayloadList<T>() ?? [];
        }

        /// <summary>
        /// Checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public override bool Exists(string key)
        {
            return _redisWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            return await _redisWrapper
                .ExistsAsync(key, 0, ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a filtered query over the items in the database.
        /// </summary>
        /// <param name="filter">Predicate that selects items of type T.</param>
        public override IEnumerable<T> Query(Func<T, bool> filter)
        {
            return _redisWrapper
                .Query(e => filter(e.ToPayloadObject<T>()))
                .Select(e => e.ToPayloadObject<T>());
        }

        /// <summary>
        /// Executes an asynchronous filtered query over the items in the database.
        /// </summary>
        /// <param name="filter">Predicate that selects items of type T.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public override async Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter, CancellationToken ct = default)
        {
            var result = await _redisWrapper
                .QueryAsync(e => filter(e.ToPayloadObject<T>()), 0, ct)
                .ConfigureAwait(false);

            return result.Select(e => e.ToPayloadObject<T>());
        }
    }
}