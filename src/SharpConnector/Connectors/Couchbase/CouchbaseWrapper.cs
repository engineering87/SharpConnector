// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Couchbase.KeyValue;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.Couchbase
{
    /// <summary>
    /// Wrapper for Couchbase operations providing simplified methods for data access and manipulation.
    /// </summary>
    public class CouchbaseWrapper : IAsyncDisposable
    {
        private readonly CouchbaseAccess _couchbaseAccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseWrapper"/> class.
        /// </summary>
        /// <param name="couchbaseConfig">The Couchbase configuration for connection setup.</param>
        public CouchbaseWrapper(CouchbaseConfig couchbaseConfig)
        {
            _couchbaseAccess = new CouchbaseAccess(couchbaseConfig);
        }

        /// <summary>
        /// Retrieves the default collection from the Couchbase bucket asynchronously.
        /// </summary>
        /// <returns>A task representing the collection of the Couchbase bucket.</returns>
        private async Task<ICouchbaseCollection> GetCollectionAsync()
        {
            var bucket = await _couchbaseAccess.GetBucketAsync();
            return await bucket.DefaultCollectionAsync();
        }

        /// <summary>
        /// Retrieves an entity by its key asynchronously.
        /// </summary>
        /// <param name="key">The key of the entity to retrieve.</param>
        /// <returns>A task representing the entity with the specified key.</returns>
        public async Task<ConnectorEntity> GetAsync(string key)
        {
            var collection = await GetCollectionAsync();
            var result = await collection.GetAsync(key);
            return result.ContentAs<ConnectorEntity>();
        }

        /// <summary>
        /// Retrieves all entities from the Couchbase bucket asynchronously.
        /// </summary>
        /// <returns>A task representing a list of all entities in the bucket.</returns>
        public async Task<IEnumerable<ConnectorEntity>> GetAllAsync()
        {
            var query = $"SELECT * FROM `{_couchbaseAccess.BucketName}`";
            var cluster = await _couchbaseAccess.GetClusterAsync();
            var result = await cluster.QueryAsync<ConnectorEntity>(query);

            return await result.Rows.ToListAsync();
        }

        /// <summary>
        /// Inserts or updates an entity in the Couchbase bucket asynchronously.
        /// </summary>
        /// <param name="entity">The entity to insert or update.</param>
        /// <returns>A task representing whether the operation succeeded.</returns>
        public async Task<bool> InsertAsync(ConnectorEntity entity)
        {
            var collection = await GetCollectionAsync();
            var result = await collection.UpsertAsync(entity.Key, entity);
            return result != null;
        }

        /// <summary>
        /// Inserts or updates multiple entities in the Couchbase bucket asynchronously.
        /// </summary>
        /// <param name="entities">The collection of entities to insert or update.</param>
        /// <returns>A task representing whether all operations succeeded.</returns>
        public async Task<bool> InsertManyAsync(IEnumerable<ConnectorEntity> entities)
        {
            var tasks = entities.Select(entity => InsertAsync(entity));
            var results = await Task.WhenAll(tasks);
            return results.All(success => success);
        }

        /// <summary>
        /// Deletes an entity by its key asynchronously.
        /// </summary>
        /// <param name="key">The key of the entity to delete.</param>
        /// <returns>A task representing whether the deletion succeeded.</returns>
        public async Task<bool> DeleteAsync(string key)
        {
            var collection = await GetCollectionAsync();
            await collection.RemoveAsync(key);
            return true;
        }

        /// <summary>
        /// Updates an entity in the Couchbase bucket asynchronously.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task representing whether the update succeeded.</returns>
        public async Task<bool> UpdateAsync(ConnectorEntity entity) => await InsertAsync(entity);

        /// <summary>
        /// Disposes the Couchbase connection asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            await _couchbaseAccess.DisposeAsync();
        }

        /// <summary>
        /// Checks whether an entity exists in the Couchbase bucket by its key.
        /// </summary>
        /// <param name="key">The key of the entity to check.</param>
        /// <returns>A boolean indicating whether the entity exists in the bucket.</returns>
        public bool Exists(string key)
        {
            var collection = GetCollectionAsync().Result;
            var result = collection.GetAsync(key).Result;
            return result != null;
        }

        /// <summary>
        /// Checks whether an entity exists in the Couchbase bucket by its key asynchronously.
        /// </summary>
        /// <param name="key">The key of the entity to check.</param>
        /// <returns>A task representing whether the entity exists in the bucket.</returns>
        public async Task<bool> ExistsAsync(string key)
        {
            var collection = await GetCollectionAsync();
            var result = await collection.GetAsync(key);
            return result != null;
        }

        public List<ConnectorEntity> Query(Func<ConnectorEntity, bool> filter)
        {
            throw new NotSupportedException();
        }

        public async Task<List<ConnectorEntity>> QueryAsync(Func<ConnectorEntity, bool> filter)
        {
            throw new NotSupportedException();
        }
    }
}
