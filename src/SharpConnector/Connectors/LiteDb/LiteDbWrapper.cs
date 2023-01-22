// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using System.Collections.Generic;

namespace SharpConnector.Connectors.LiteDb
{
    public class LiteDbWrapper
    {
        private readonly LiteDbAccess _liteDbAccess;

        /// <summary>
        /// Create a new LiteDbWrapper instance.
        /// </summary>
        /// <param name="liteDbConfig">The LiteDbConfig connector config.</param>
        public LiteDbWrapper(LiteDbConfig liteDbConfig)
        {
            _liteDbAccess = new LiteDbAccess(liteDbConfig);
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public LiteDbConnectorEntity Get(string key)
        {
            return _liteDbAccess.Collection.Find(Query.EQ("Key", new BsonValue(key))).FirstOrDefault();
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public Task<LiteDbConnectorEntity> GetAsync(string key)
        {
            // LiteDb library does not implement asynchronous operations
            var entity = _liteDbAccess.Collection.Find(Query.EQ("Key", new BsonValue(key))).FirstOrDefault();
            return Task.FromResult(entity);
        }

        /// <summary>
        /// Get all the values.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LiteDbConnectorEntity> GetAll()
        {
            return _liteDbAccess.Collection.Find(x=> true).ToList();
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public bool Insert(LiteDbConnectorEntity connectorEntity)
        {
            var result = _liteDbAccess.Collection.Insert(connectorEntity);
            return !result.IsNull;
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public Task<bool> InsertAsync(LiteDbConnectorEntity connectorEntity)
        {
            // LiteDb library does not implement asynchronous operations
            Delete(connectorEntity.Key);
            var insert = Insert(connectorEntity);
            return Task.FromResult(insert);
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="connectorEntities">The ConnectorEntities to store.</param>
        /// <returns></returns>
        public bool InsertMany(List<LiteDbConnectorEntity> connectorEntities)
        {
            _liteDbAccess.Collection.InsertBulk(connectorEntities);
            return true;
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            return _liteDbAccess.Collection.DeleteMany(Query.EQ("Key", new BsonValue(key))) > 0;
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(string key)
        {
            // LiteDb library does not implement asynchronous operations yet
            var delete = Delete(key);
            return Task.FromResult(delete);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to update.</param>
        /// <returns></returns>
        public bool Update(LiteDbConnectorEntity connectorEntity)
        {
            // TODO switch to single access with UpdateMany
            _liteDbAccess.Collection.DeleteMany(Query.EQ("Key", new BsonValue(connectorEntity.Key)));
            return Insert(connectorEntity);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to update.</param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(LiteDbConnectorEntity connectorEntity)
        {
            // LiteDb library does not implement asynchronous operations yet
            var update = Update(connectorEntity);
            return Task.FromResult(update);
        }
    }
}
