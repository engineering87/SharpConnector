// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using MongoDB.Driver;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public MongoConnectorEntity Get(string key)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq("Key", key);
            
            return _mongoDbAccess.Collection
                .Find(filter)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public async Task<MongoConnectorEntity> GetAsync(string key)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, key);
            var cursor = await _mongoDbAccess.Collection.FindAsync(filter).ConfigureAwait(false);
            
            return await cursor.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get all the values.
        /// </summary>
        /// <returns></returns>
        public List<MongoConnectorEntity> GetAll()
        {
            return _mongoDbAccess.Collection
                .Find(x => true)
                .ToList();
        }

        /// <summary>
        /// Asynchronously get all the values.
        /// </summary>
        /// <returns></returns>
        public async Task<List<MongoConnectorEntity>> GetAllAsync()
        {
            return await _mongoDbAccess.Collection
                .Find(FilterDefinition<MongoConnectorEntity>.Empty)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public bool Insert(MongoConnectorEntity connectorEntity)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, connectorEntity.Key);
            var options = new ReplaceOptions { IsUpsert = true };

            var result = _mongoDbAccess.Collection.ReplaceOne(filter, connectorEntity, options);
            return result.IsAcknowledged;
        }

        /// <summary>
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(MongoConnectorEntity connectorEntity)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq("Key", connectorEntity.Key);
            var options = new ReplaceOptions { IsUpsert = true };

            var result = await _mongoDbAccess.Collection.ReplaceOneAsync(filter, connectorEntity, options).ConfigureAwait(false);
            return result.IsAcknowledged;
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="connectorEntities">The ConnectorEntities to store.</param>
        /// <returns></returns>
        public bool InsertMany(List<MongoConnectorEntity> connectorEntities)
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
        public async Task<bool> InsertManyAsync(List<MongoConnectorEntity> connectorEntities)
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
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, key);
            
            var result = _mongoDbAccess.Collection.DeleteOne(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <summary>
        /// Asynchronously removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object to delete.</param>
        /// <returns>A boolean indicating if the deletion was acknowledged.</returns>
        public async Task<bool> DeleteAsync(string key)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, key);
            
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
        public bool Update(MongoConnectorEntity connectorEntity)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, connectorEntity.Key);
            var update = Builders<MongoConnectorEntity>.Update
                .Set(x => x.Payload, connectorEntity.Payload);

            var result = _mongoDbAccess.Collection.UpdateOne(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns>A boolean indicating if the update was acknowledged.</returns>
        public async Task<bool> UpdateAsync(MongoConnectorEntity connectorEntity)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, connectorEntity.Key);
            var update = Builders<MongoConnectorEntity>.Update
                .Set(x => x.Payload, connectorEntity.Payload);

            var result = await _mongoDbAccess.Collection
                .UpdateOneAsync(filter, update)
                .ConfigureAwait(false);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public bool Exists(string key)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, key);
            return _mongoDbAccess.Collection
                .Find(filter)
                .Any();
        }

        /// <summary>
        /// Asynchronously checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>A task containing true if the item exists, false otherwise.</returns>
        public async Task<bool> ExistsAsync(string key)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, key);
            return await _mongoDbAccess.Collection
                .Find(filter)
                .AnyAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Queries MongoDB collection with a filter function.
        /// </summary>
        /// <param name="filter">A filter definition to apply to the query.</param>
        /// <returns>A collection of filtered MongoConnectorEntity instances.</returns>
        public List<MongoConnectorEntity> Query(Func<MongoConnectorEntity, bool> filter)
        {
            return _mongoDbAccess.Collection
                .AsQueryable()
                .Where(filter)
                .ToList();
        }

        /// <summary>
        /// Asynchronously queries MongoDB collection with a filter function.
        /// </summary>
        /// <param name="filter">A filter definition to apply to the query.</param>
        /// <returns>A task containing a collection of filtered MongoConnectorEntity instances.</returns>
        public async Task<List<MongoConnectorEntity>> QueryAsync(Func<MongoConnectorEntity, bool> filter)
        {
            // TODO this is not efficient, but it's a dirty way to get the job done
            var allDocuments = await _mongoDbAccess.Collection
                .Find(FilterDefinition<MongoConnectorEntity>.Empty)
                .ToListAsync();

            return allDocuments
                .Where(filter)
                .ToList();
        }
    }
}
