// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using MongoDB.Driver;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System.Collections.Generic;
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
            return _mongoDbAccess.Collection
                .Find(filter)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public async Task<ConnectorEntity> GetAsync(string key)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", key);
            var entity = await _mongoDbAccess.Collection.FindAsync(filter);
            return await entity.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get all the values.
        /// </summary>
        /// <returns></returns>
        public List<ConnectorEntity> GetAll()
        {
            return _mongoDbAccess.Collection
                .Find(x => true)
                .ToList();
        }

        /// <summary>
        /// Asynchronously get all the values.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ConnectorEntity>> GetAllAsync()
        {
            return await _mongoDbAccess.Collection
                .Find(x => true)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public bool Insert(ConnectorEntity connectorEntity)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", connectorEntity.Key);
            var options = new ReplaceOptions { IsUpsert = true };
            _mongoDbAccess.Collection
                .ReplaceOne(filter, connectorEntity, options);
            return true;
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(ConnectorEntity connectorEntity)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", connectorEntity.Key);
            var options = new ReplaceOptions { IsUpsert = true };
            await _mongoDbAccess.Collection
                .ReplaceOneAsync(filter, connectorEntity, options)
                .ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="connectorEntities">The ConnectorEntities to store.</param>
        /// <returns></returns>
        public bool InsertMany(List<ConnectorEntity> connectorEntities)
        {
            var options = new InsertManyOptions { IsOrdered = false };
            _mongoDbAccess.Collection
                .InsertMany(connectorEntities, options);
            return true;
        }

        /// <summary>
        /// Asynchronously inserts multiple ConnectorEntities.
        /// </summary>
        /// <param name="connectorEntities">The list of ConnectorEntities to store.</param>
        /// <returns>A boolean indicating if the operation completed successfully.</returns>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities)
        {
            var options = new InsertManyOptions { IsOrdered = false };
            await _mongoDbAccess.Collection
                .InsertManyAsync(connectorEntities, options)
                .ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", key);
            return _mongoDbAccess.Collection
                .DeleteOne(filter).IsAcknowledged;
        }

        /// <summary>
        /// Asynchronously removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object to delete.</param>
        /// <returns>A boolean indicating if the deletion was acknowledged.</returns>
        public async Task<bool> DeleteAsync(string key)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", key);
            var result = await _mongoDbAccess.Collection
                .DeleteOneAsync(filter)
                .ConfigureAwait(false);
            return result.IsAcknowledged && result.DeletedCount > 0;
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
            return _mongoDbAccess.Collection
                .UpdateOne(filter, update).IsAcknowledged;
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns>A boolean indicating if the update was acknowledged.</returns>
        public async Task<bool> UpdateAsync(ConnectorEntity connectorEntity)
        {
            var filter = Builders<ConnectorEntity>.Filter.Eq("Key", connectorEntity.Key);
            var update = Builders<ConnectorEntity>.Update.Set("Payload", connectorEntity.Payload);
            var result = await _mongoDbAccess.Collection
                .UpdateOneAsync(filter, update)
                .ConfigureAwait(false);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
