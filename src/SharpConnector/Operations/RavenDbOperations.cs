// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using SharpConnector.Connectors.RavenDb;
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpConnector.Operations
{
    public class RavenDbOperations<T> : Operations<T>
    {
        private readonly RavenDbWrapper _ravenDbWrapper;

        /// <summary>
        /// Create a new MongoDbOperations instance.
        /// </summary>
        /// <param name="mongoDbConfig">The MongoDb connector config.</param>
        public RavenDbOperations(RavenDbConfig ravenDbConfig)
        {
            _ravenDbWrapper = new RavenDbWrapper(ravenDbConfig);
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override T Get(string key)
        {
            var connectorEntity = _ravenDbWrapper.Get(key);
            if (connectorEntity != null)
                return (T)Convert.ChangeType(connectorEntity.Payload, typeof(T));
            return default;
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override Task<T> GetAsync(string key)
        {
            var connectorEntity = _ravenDbWrapper.GetAsync(key);
            if (connectorEntity != null)
                return (Task<T>)Convert.ChangeType(connectorEntity.Result.Payload, typeof(Task<T>));
            return default;
        }

        public override IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
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
            return _ravenDbWrapper.Insert(connectorEntity);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override bool Insert(string key, T value, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override Task<bool> InsertAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _ravenDbWrapper.InsertAsync(connectorEntity);
        }

        public override Task<bool> InsertAsync(string key, T value, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override bool Delete(string key)
        {
            return _ravenDbWrapper.Delete(key);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public override Task<bool> DeleteAsync(string key)
        {
            return _ravenDbWrapper.DeleteAsync(key);
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
            return _ravenDbWrapper.Update(connectorEntity);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to store.</param>
        /// <returns></returns>
        public override Task<bool> UpdateAsync(string key, T value)
        {
            var connectorEntity = new ConnectorEntity(key, value, null);
            return _ravenDbWrapper.UpdateAsync(connectorEntity);
        }
    }
}
