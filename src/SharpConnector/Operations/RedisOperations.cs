// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <returns></returns>
        public override T Get(string key)
        {
            var connectorEntity = _redisWrapper.Get(key);
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
            var connectorEntity = await _redisWrapper.GetAsync(key);
            if (connectorEntity != null)
                return connectorEntity.ToPayloadObject<T>();
            return default;
        }

        /// <summary>
        /// Get all values from Redis database.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<T> GetAll()
        {
            var connectorEntities = _redisWrapper.GetAll();
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
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _redisWrapper.Insert(connectorEntity);
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
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return _redisWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override async Task<bool> InsertAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _redisWrapper.InsertAsync(connectorEntity);
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
            var connectorEntity = new ConnectorEntity(key, value, expiration);
            return await _redisWrapper.InsertAsync(connectorEntity);
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <returns></returns>
        public override bool InsertMany(Dictionary<string, T> values)
        {
            return _redisWrapper.InsertMany(values.ToConnectorEntityList());
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="values">The values to store.</param>
        /// <param name="expiration">The expiration of the keys.</param>
        /// <returns></returns>
        public override bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _redisWrapper.InsertMany(values.ToConnectorEntityList(expiration));
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override bool Delete(string key)
        {
            return _redisWrapper.Delete(key);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override async Task<bool> DeleteAsync(string key)
        {
            return await _redisWrapper.DeleteAsync(key);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override bool Update(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _redisWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return await _redisWrapper.UpdateAsync(connectorEntity);
        }

        /// <summary>
        /// Get all values from Redis asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with the result being all values stored in Redis.</returns>
        public override async Task<IEnumerable<T>> GetAllAsync()
        {
            var connectorEntities = await _redisWrapper.GetAllAsync();
            return connectorEntities
                .Cast<T>()
                .ToList();
        }

        /// <summary>
        /// Insert multiple key-value pairs into Redis asynchronously.
        /// </summary>
        /// <param name="values">A collection of values to store.</param>
        /// <returns>A task representing the asynchronous operation, with the result being true if all insertions are successful.</returns>
        public override async Task<bool> InsertManyAsync(IEnumerable<T> values)
        {
            var connectorEntityList = values
                .Cast<ConnectorEntity>()
                .ToList();
            return await _redisWrapper.InsertManyAsync(connectorEntityList);
        }

        /// <summary>
        /// Checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public override bool Exists(string key)
        {
            return _redisWrapper.Exists(key);
        }

        /// <summary>
        /// Asynchronously checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>A task containing true if the item exists, false otherwise.</returns>
        public override async Task<bool> ExistsAsync(string key)
        {
            return await _redisWrapper.ExistsAsync(key);
        }

        /// <summary>
        /// Esegue una query filtrata sugli elementi nel database.
        /// </summary>
        /// <param name="filter">Funzione di filtro che seleziona gli elementi di tipo T.</param>
        /// <returns>Un IEnumerable di elementi di tipo T che soddisfano il filtro.</returns>
        public override IEnumerable<T> Query(Func<T, bool> filter)
        {
            bool castedFilter(ConnectorEntity entity) =>
                entity is T tItem && filter(tItem);

            return _redisWrapper
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
            bool castedFilter(ConnectorEntity entity) =>
                entity is T tItem && filter(tItem);

            var result = await _redisWrapper.QueryAsync(castedFilter);
            return result.Cast<T>();
        }
    }
}
