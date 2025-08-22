// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using ArangoDBNetStandard.CollectionApi.Models;
using ArangoDBNetStandard.CursorApi.Models;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.ArangoDb
{
    public class ArangoDbWrapper : IDisposable
    {
        private readonly ArangoDbAccess _arangoDbAccess;
        private readonly string _collectionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArangoDbWrapper"/> class.
        /// </summary>
        /// <param name="arangoDbConfig">The ArangoDB configuration settings.</param>
        public ArangoDbWrapper(ArangoDbConfig arangoDbConfig)
        {
            _arangoDbAccess = new ArangoDbAccess(arangoDbConfig);
            _collectionName = arangoDbConfig.CollectionName;
        }

        /// <summary>
        /// Ensure the target collection exists in the database.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task EnsureCollectionExistsAsync(CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var collections = await _arangoDbAccess.Client.Collection
                .GetCollectionsAsync()
                .ConfigureAwait(false);

            if (!collections.Result.Any(c => c.Name == _collectionName))
            {
                ct.ThrowIfCancellationRequested();

                await _arangoDbAccess.Client.Collection
                    .PostCollectionAsync(new PostCollectionBody { Name = _collectionName })
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieve a document by key (sync wrapper).
        /// </summary>
        /// <param name="key">The document key.</param>
        public ConnectorEntity Get(string key)
        {
            return GetAsync(key, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Retrieve a document by key (async).
        /// </summary>
        /// <param name="key">The document key.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<ConnectorEntity> GetAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var doc = await _arangoDbAccess.Client.Document
                .GetDocumentAsync<ConnectorEntity>(_collectionName, key, token: ct)
                .ConfigureAwait(false);

            return doc;
        }

        /// <summary>
        /// Retrieve all documents (sync wrapper).
        /// </summary>
        public List<ConnectorEntity> GetAll()
        {
            return GetAllAsync(CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Retrieve all documents (async).
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<List<ConnectorEntity>> GetAllAsync(CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var first = await _arangoDbAccess.Client.Cursor
                .PostCursorAsync<ConnectorEntity>(new PostCursorBody
                {
                    Query = $"FOR doc IN {_collectionName} RETURN doc"
                }, token: ct)
                .ConfigureAwait(false);

            var items = new List<ConnectorEntity>();
            if (first.Result != null) items.AddRange(first.Result);

            var cursorId = first.Id;
            var hasMore = first.HasMore == true;

            while (hasMore && !string.IsNullOrEmpty(cursorId))
            {
                ct.ThrowIfCancellationRequested();

                var next = await _arangoDbAccess.Client.Cursor
                    .PutCursorAsync<ConnectorEntity>(cursorId, token: ct)
                    .ConfigureAwait(false);

                if (next.Result != null) items.AddRange(next.Result);

                cursorId = next.Id;
                hasMore = next.HasMore == true;
            }

            return items;
        }

        /// <summary>
        /// Insert a single document (sync wrapper).
        /// </summary>
        /// <param name="connectorEntity">The document to insert.</param>
        public bool Insert(ConnectorEntity connectorEntity)
        {
            return InsertAsync(connectorEntity, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Insert a single document (async).
        /// </summary>
        /// <param name="connectorEntity">The document to insert.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> InsertAsync(ConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var result = await _arangoDbAccess.Client.Document
                .PostDocumentAsync(_collectionName, connectorEntity, token: ct)
                .ConfigureAwait(false);

            return !string.IsNullOrEmpty(result._id);
        }

        /// <summary>
        /// Insert multiple documents (sync wrapper).
        /// </summary>
        /// <param name="connectorEntities">The documents to insert.</param>
        public bool InsertMany(List<ConnectorEntity> connectorEntities)
        {
            return InsertManyAsync(connectorEntities, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Insert multiple documents (async).
        /// </summary>
        /// <param name="connectorEntities">The documents to insert.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var result = await _arangoDbAccess.Client.Document
                .PostDocumentsAsync(_collectionName, connectorEntities, token: ct)
                .ConfigureAwait(false);

            ct.ThrowIfCancellationRequested();
            return result.All(r => !string.IsNullOrEmpty(r._id));
        }

        /// <summary>
        /// Update a document (sync wrapper).
        /// </summary>
        /// <param name="connectorEntity">The document to update.</param>
        public bool Update(ConnectorEntity connectorEntity)
        {
            return UpdateAsync(connectorEntity, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Update a document (async).
        /// </summary>
        /// <param name="connectorEntity">The document to update.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> UpdateAsync(ConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            await _arangoDbAccess.Client.Document
                .PutDocumentAsync(_collectionName, connectorEntity.Key, connectorEntity, token: ct)
                .ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Delete a document by key (sync wrapper).
        /// </summary>
        /// <param name="key">The key of the document to delete.</param>
        public bool Delete(string key)
        {
            return DeleteAsync(key, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Delete a document by key (async).
        /// </summary>
        /// <param name="key">The key of the document to delete.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            await _arangoDbAccess.Client.Document
                .DeleteDocumentAsync<ConnectorEntity>(_collectionName, key, token: ct)
                .ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Dispose database access resources.
        /// </summary>
        public void Dispose()
        {
            _arangoDbAccess.Dispose();
        }

        /// <summary>
        /// Check whether a document exists by key (sync wrapper).
        /// </summary>
        /// <param name="key">The document key.</param>
        public bool Exists(string key)
        {
            return ExistsAsync(key, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Check whether a document exists by key (async).
        /// </summary>
        /// <param name="key">The document key.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var result = await _arangoDbAccess.Client.Document
                .GetDocumentAsync<ConnectorEntity>(_collectionName, key, token: ct)
                .ConfigureAwait(false);

            return result != null;
        }

        /// <summary>
        /// Execute a query and filter the results in memory (sync wrapper).
        /// </summary>
        /// <param name="filter">Predicate used to filter items.</param>
        public List<ConnectorEntity> Query(Func<ConnectorEntity, bool> filter)
        {
            return QueryAsync(filter, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Execute a query and filter the results in memory (async).
        /// </summary>
        /// <param name="filter">Predicate used to filter items.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<List<ConnectorEntity>> QueryAsync(Func<ConnectorEntity, bool> filter, CancellationToken ct = default)
        {
            var entities = await GetAllAsync(ct).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
            return entities
                .Where(filter)
                .ToList();
        }
    }
}