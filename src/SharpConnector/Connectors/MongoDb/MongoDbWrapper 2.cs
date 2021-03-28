// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using MongoDB.Driver;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.MongoDb
{
    public class MongoDbWrapper
    {
        private readonly MongoDbAccess _mongoDbAccess;

        /// <summary>
        /// Create a new MongoDbWrapper instance.
        /// </summary>
        /// <param name="mongoDbConfig">The MongoDb connector config.</param>
        public MongoDbWrapper(MongoDbConfig mongoDbConfig)
        {
            _mongoDbAccess = new MongoDbAccess(mongoDbConfig);
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public ConnectorEntity Get(string key)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", key);
            return _mongoDbAccess.Collection.Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public Task<ConnectorEntity> GetAsync(string key)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", key);
            return _mongoDbAccess.Collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public bool Insert(ConnectorEntity connectorEntity)
        {
            Delete(connectorEntity.Key);
            _mongoDbAccess.Collection.InsertOne(connectorEntity);
            return true;
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public Task<bool> InsertAsync(ConnectorEntity connectorEntity)
        {
            DeleteAsync(connectorEntity.Key);
            var insert = _mongoDbAccess.Collection.InsertOneAsync(connectorEntity);
            return Task.FromResult(insert.IsCompletedSuccessfully);
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", key);
            return _mongoDbAccess.Collection.DeleteOne(filter).IsAcknowledged;
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(string key)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", key);
            var delete = _mongoDbAccess.Collection.DeleteOneAsync(filter);
            return Task.FromResult(delete.IsCompletedSuccessfully);
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public bool Update(ConnectorEntity connectorEntity)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", connectorEntity.Key);
            var update = Builders<ConnectorEntity>.Update.Set("Payload", connectorEntity.Payload);
            return _mongoDbAccess.Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(ConnectorEntity connectorEntity)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", connectorEntity.Key);
            var update = Builders<ConnectorEntity>.Update.Set("Payload", connectorEntity.Payload);
            return Task.FromResult(_mongoDbAccess.Collection.UpdateOneAsync(filter, update).IsCompletedSuccessfully);
        }
    }
}
