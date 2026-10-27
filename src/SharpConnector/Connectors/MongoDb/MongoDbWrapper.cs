// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using MongoDB.Driver;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.MongoDb
{
    public class MongoDbWrapper : IDisposable
    {
        private readonly MongoDbAccess _mongoDbAccess;

        /// <summary>
        /// Create a new MongoDbWrapper instance.
        /// </summary>
        /// <param name="mongoDbConfig">The MongoDB connector configuration.</param>
        public MongoDbWrapper(MongoDbConfig mongoDbConfig)
        {
            _mongoDbAccess = new MongoDbAccess(mongoDbConfig);
        }

        /// <summary>
        /// Disposes the underlying MongoDB client.
        /// </summary>
        public void Dispose()
        {
            _mongoDbAccess?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public MongoConnectorEntity Get(string key)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, key);

            return _mongoDbAccess.Collection
                .Find(filter)
                .FirstOrDefault()
                .DiscardIfExpired();
        }

        /// <summary>
        /// Asynchronously retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<MongoConnectorEntity> GetAsync(string key, CancellationToken ct = default)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, key);

            var entity = await _mongoDbAccess.Collection
                .Find(filter)
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);

            return entity.DiscardIfExpired();
        }

        /// <summary>
        /// Retrieve all values.
        /// </summary>
        public List<MongoConnectorEntity> GetAll()
        {
            return _mongoDbAccess.Collection
                .Find(FilterDefinition<MongoConnectorEntity>.Empty)
                .ToList()
                .WhereNotExpired()
                .ToList();
        }

        /// <summary>
        /// Asynchronously retrieve all values.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<List<MongoConnectorEntity>> GetAllAsync(CancellationToken ct = default)
        {
            var entities = await _mongoDbAccess.Collection
                .Find(FilterDefinition<MongoConnectorEntity>.Empty)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            return entities.WhereNotExpired().ToList();
        }

        /// <summary>
        /// Set the key to hold the specified value (upsert).
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        public bool Insert(MongoConnectorEntity connectorEntity)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, connectorEntity.Key);
            var options = new ReplaceOptions { IsUpsert = true };

            var result = _mongoDbAccess.Collection.ReplaceOne(filter, connectorEntity, options);
            return result.IsAcknowledged;
        }

        /// <summary>
        /// Asynchronously set the key to hold the specified value (upsert).
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> InsertAsync(MongoConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, connectorEntity.Key);
            var options = new ReplaceOptions { IsUpsert = true };

            var result = await _mongoDbAccess.Collection
                .ReplaceOneAsync(filter, connectorEntity, options, ct)
                .ConfigureAwait(false);

            return result.IsAcknowledged;
        }

        /// <summary>
        /// Insert multiple entities.
        /// </summary>
        /// <param name="connectorEntities">The entities to store.</param>
        public bool InsertMany(List<MongoConnectorEntity> connectorEntities)
        {
            var requests = BuildUpsertRequests(connectorEntities);
            if (requests.Count == 0)
                return true;

            var result = _mongoDbAccess.Collection.BulkWrite(requests, new BulkWriteOptions { IsOrdered = false });
            return result.IsAcknowledged;
        }

        /// <summary>
        /// Builds one upsert (replace by Key) request per entity, so repeated keys never create duplicates.
        /// </summary>
        internal static List<WriteModel<MongoConnectorEntity>> BuildUpsertRequests(IEnumerable<MongoConnectorEntity> connectorEntities)
        {
            ArgumentNullException.ThrowIfNull(connectorEntities);

            return connectorEntities
                .Select(e => (WriteModel<MongoConnectorEntity>)new ReplaceOneModel<MongoConnectorEntity>(
                    Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, e.Key), e)
                { IsUpsert = true })
                .ToList();
        }

        /// <summary>
        /// Asynchronously insert multiple entities.
        /// </summary>
        /// <param name="connectorEntities">The entities to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the insert completed successfully; otherwise, false.</returns>
        public async Task<bool> InsertManyAsync(List<MongoConnectorEntity> connectorEntities, CancellationToken ct = default)
        {
            var requests = BuildUpsertRequests(connectorEntities);
            if (requests.Count == 0)
                return true;

            var result = await _mongoDbAccess.Collection
                .BulkWriteAsync(requests, new BulkWriteOptions { IsOrdered = false }, ct)
                .ConfigureAwait(false);

            return result.IsAcknowledged;
        }

        /// <summary>
        /// Remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public bool Delete(string key)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, key);

            var result = _mongoDbAccess.Collection.DeleteOne(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <summary>
        /// Asynchronously remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the deletion was acknowledged and affected a document; otherwise, false.</returns>
        public async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, key);

            var result = await _mongoDbAccess.Collection
                .DeleteOneAsync(filter, ct)
                .ConfigureAwait(false);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <summary>
        /// Update the specified key's payload.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        public bool Update(MongoConnectorEntity connectorEntity)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, connectorEntity.Key);
            var update = Builders<MongoConnectorEntity>.Update
                .Set(x => x.Payload, connectorEntity.Payload)
                .Set(x => x.Expiration, connectorEntity.Expiration)
                .Set(x => x.ExpiresAt, connectorEntity.ExpiresAt);

            var result = _mongoDbAccess.Collection.UpdateOne(filter, update);
            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        /// <summary>
        /// Asynchronously update the specified key's payload.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the update was acknowledged and matched a document; otherwise, false.</returns>
        public async Task<bool> UpdateAsync(MongoConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            var filter = Builders<MongoConnectorEntity>.Filter.Eq(x => x.Key, connectorEntity.Key);
            var update = Builders<MongoConnectorEntity>.Update
                .Set(x => x.Payload, connectorEntity.Payload)
                .Set(x => x.Expiration, connectorEntity.Expiration)
                .Set(x => x.ExpiresAt, connectorEntity.ExpiresAt);

            var result = await _mongoDbAccess.Collection
                .UpdateOneAsync(filter, update, options: null, ct)
                .ConfigureAwait(false);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        /// <summary>
        /// Check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public bool Exists(string key)
        {
            return Get(key) != null;
        }

        /// <summary>
        /// Asynchronously check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the item exists; otherwise, false.</returns>
        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            return await GetAsync(key, ct).ConfigureAwait(false) != null;
        }

        /// <summary>
        /// Query the MongoDB collection with an in-memory filter function.
        /// </summary>
        /// <param name="filter">A function used to filter the results in memory.</param>
        /// <returns>A collection of filtered <see cref="MongoConnectorEntity"/> instances.</returns>
        public List<MongoConnectorEntity> Query(Func<MongoConnectorEntity, bool> filter)
        {
            return GetAll()
                .Where(filter)
                .ToList();
        }

        /// <summary>
        /// Asynchronously query the MongoDB collection with an in-memory filter function.
        /// </summary>
        /// <param name="filter">A function used to filter the results in memory.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task whose result is a collection of filtered <see cref="MongoConnectorEntity"/> instances.</returns>
        public async Task<List<MongoConnectorEntity>> QueryAsync(Func<MongoConnectorEntity, bool> filter, CancellationToken ct = default)
        {
            var allDocuments = await GetAllAsync(ct).ConfigureAwait(false);

            return allDocuments
                .Where(filter)
                .ToList();
        }
    }
}