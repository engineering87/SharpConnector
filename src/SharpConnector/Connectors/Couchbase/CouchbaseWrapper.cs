// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Couchbase.Query;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the collection of the Couchbase bucket.</returns>
        private async Task<ICouchbaseCollection> GetCollectionAsync(CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var bucket = await _couchbaseAccess.GetBucketAsync().ConfigureAwait(false);
            return await bucket.DefaultCollectionAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves an entity by its key asynchronously.
        /// </summary>
        /// <param name="key">The key of the entity to retrieve.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the entity with the specified key.</returns>
        public async Task<ConnectorEntity> GetAsync(string key, CancellationToken ct = default)
        {
            try
            {
                var collection = await GetCollectionAsync(ct).ConfigureAwait(false);
                var result = await collection.GetAsync(key, new GetOptions().CancellationToken(ct)).ConfigureAwait(false);
                return result.ContentAs<ConnectorEntity>();
            }
            catch (DocumentNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves all entities from the Couchbase bucket asynchronously.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing a list of all entities in the bucket.</returns>
        public async Task<IEnumerable<ConnectorEntity>> GetAllAsync(CancellationToken ct = default)
        {
            var query = $"SELECT RAW doc FROM `{_couchbaseAccess.BucketName}` AS doc";
            var cluster = await _couchbaseAccess.GetClusterAsync().ConfigureAwait(false);
            var result = await cluster.QueryAsync<ConnectorEntity>(query, new QueryOptions().CancellationToken(ct))
                                      .ConfigureAwait(false);

            return await result.Rows.ToListAsync(ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Inserts or updates an entity in the Couchbase bucket asynchronously.
        /// </summary>
        /// <param name="entity">The entity to insert or update.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing whether the operation succeeded.</returns>
        public async Task<bool> InsertAsync(ConnectorEntity entity, CancellationToken ct = default)
        {
            var collection = await GetCollectionAsync(ct).ConfigureAwait(false);
            var options = new UpsertOptions().CancellationToken(ct);
            var result = await collection.UpsertAsync(entity.Key, entity, options).ConfigureAwait(false);
            return result is not null;
        }

        /// <summary>
        /// Inserts or updates multiple entities in the Couchbase bucket asynchronously.
        /// </summary>
        /// <param name="entities">The collection of entities to insert or update.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing whether all operations succeeded.</returns>
        public async Task<bool> InsertManyAsync(IEnumerable<ConnectorEntity> entities, CancellationToken ct = default)
        {
            var tasks = new List<Task<bool>>();
            foreach (var entity in entities)
            {
                ct.ThrowIfCancellationRequested();
                tasks.Add(InsertAsync(entity, ct));
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            return results.All(success => success);
        }

        /// <summary>
        /// Deletes an entity by its key asynchronously.
        /// </summary>
        /// <param name="key">The key of the entity to delete.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing whether the deletion succeeded.</returns>
        public async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            try
            {
                var collection = await GetCollectionAsync(ct).ConfigureAwait(false);
                await collection.RemoveAsync(key, new RemoveOptions().CancellationToken(ct)).ConfigureAwait(false);
                return true;
            }
            catch (DocumentNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Updates an entity in the Couchbase bucket asynchronously (Upsert).
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing whether the update succeeded.</returns>
        public Task<bool> UpdateAsync(ConnectorEntity entity, CancellationToken ct = default) =>
            InsertAsync(entity, ct);

        /// <summary>
        /// Disposes the Couchbase connection asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            await _couchbaseAccess.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Checks whether an entity exists in the Couchbase bucket by its key (synchronous wrapper).
        /// </summary>
        /// <param name="key">The key of the entity to check.</param>
        /// <returns>True if the entity exists; otherwise, false.</returns>
        public bool Exists(string key)
        {
            var collection = GetCollectionAsync(CancellationToken.None).GetAwaiter().GetResult();
            var result = collection.ExistsAsync(key).GetAwaiter().GetResult();
            return result.Exists;
        }

        /// <summary>
        /// Checks whether an entity exists in the Couchbase bucket by its key asynchronously.
        /// </summary>
        /// <param name="key">The key of the entity to check.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing whether the entity exists in the bucket.</returns>
        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            var collection = await GetCollectionAsync(ct).ConfigureAwait(false);
            var result = await collection.ExistsAsync(key, new ExistsOptions().CancellationToken(ct))
                                        .ConfigureAwait(false);
            return result.Exists;
        }

        /// <summary>
        /// Synchronously retrieves a list of <see cref="ConnectorEntity"/> objects that match the specified filter.
        /// (Runs a N1QL query under the hood.)
        /// </summary>
        /// <param name="filter">A function to test each element for a condition.</param>
        public List<ConnectorEntity> Query(Func<ConnectorEntity, bool> filter)
        {
            return QueryAsync(filter, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Asynchronously retrieves a list of <see cref="ConnectorEntity"/> objects that match the specified filter.
        /// (Runs a N1QL query under the hood and filters in memory.)
        /// </summary>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<List<ConnectorEntity>> QueryAsync(Func<ConnectorEntity, bool> filter, CancellationToken ct = default)
        {
            var query = $"SELECT RAW doc FROM `{_couchbaseAccess.BucketName}` AS doc";
            var cluster = await _couchbaseAccess.GetClusterAsync().ConfigureAwait(false);
            var result = await cluster.QueryAsync<ConnectorEntity>(query, new QueryOptions().CancellationToken(ct))
                                      .ConfigureAwait(false);

            var rows = await result.Rows.ToListAsync(ct).ConfigureAwait(false);
            return rows.Where(filter).ToList();
        }
    }
}