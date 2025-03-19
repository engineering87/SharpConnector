// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using ArangoDBNetStandard.CollectionApi.Models;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Ensures the target collection exists in the database.
        /// </summary>
        public async Task EnsureCollectionExistsAsync()
        {
            var collections = await _arangoDbAccess.Client.Collection.GetCollectionsAsync();
            if (!collections.Result.Any(c => c.Name == _collectionName))
            {
                await _arangoDbAccess.Client.Collection.PostCollectionAsync(new PostCollectionBody
                {
                    Name = _collectionName
                });
            }
        }

        /// <summary>
        /// Retrieves a document by key.
        /// </summary>
        /// <param name="key">The document key.</param>
        /// <returns>The document as a <see cref="ConnectorEntity"/>.</returns>
        public ConnectorEntity Get(string key)
        {
            var task = _arangoDbAccess.Client.Document.GetDocumentAsync<ConnectorEntity>(_collectionName, key);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Asynchronously retrieves a document by key.
        /// </summary>
        /// <param name="key">The document key.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the document.</returns>
        public async Task<ConnectorEntity> GetAsync(string key)
        {
            return await _arangoDbAccess.Client.Document.GetDocumentAsync<ConnectorEntity>(_collectionName, key);
        }

        /// <summary>
        /// Retrieves all documents from the collection.
        /// </summary>
        /// <returns>A list of <see cref="ConnectorEntity"/>.</returns>
        public List<ConnectorEntity> GetAll()
        {
            var task = GetAllAsync();
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Asynchronously retrieves all documents from the collection.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of documents.</returns>
        public async Task<List<ConnectorEntity>> GetAllAsync()
        {
            var cursor = await _arangoDbAccess.Client.Cursor.PostCursorAsync<ConnectorEntity>(new ArangoDBNetStandard.CursorApi.Models.PostCursorBody
            {
                Query = $"FOR doc IN {_collectionName} RETURN doc"
            });

            return cursor.Result?.ToList() ?? [];
        }

        /// <summary>
        /// Inserts a single document into the collection.
        /// </summary>
        /// <param name="connectorEntity">The document to insert.</param>
        /// <returns>True if insertion was successful.</returns>
        public bool Insert(ConnectorEntity connectorEntity)
        {
            var task = InsertAsync(connectorEntity);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Asynchronously inserts a single document into the collection.
        /// </summary>
        /// <param name="connectorEntity">The document to insert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public async Task<bool> InsertAsync(ConnectorEntity connectorEntity)
        {
            var result = await _arangoDbAccess.Client.Document.PostDocumentAsync(_collectionName, connectorEntity);
            return !string.IsNullOrEmpty(result._id);
        }

        /// <summary>
        /// Inserts multiple documents into the collection.
        /// </summary>
        /// <param name="connectorEntities">The list of documents to insert.</param>
        /// <returns>True if all insertions were successful.</returns>
        public bool InsertMany(List<ConnectorEntity> connectorEntities)
        {
            var task = InsertManyAsync(connectorEntities);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Asynchronously inserts multiple documents into the collection.
        /// </summary>
        /// <param name="connectorEntities">The list of documents to insert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities)
        {
            var result = await _arangoDbAccess.Client.Document.PostDocumentsAsync(_collectionName, connectorEntities);
            return result.All(r => !string.IsNullOrEmpty(r._id));
        }

        /// <summary>
        /// Updates a document in the collection.
        /// </summary>
        /// <param name="connectorEntity">The document to update.</param>
        /// <returns>True if the update was successful.</returns>
        public bool Update(ConnectorEntity connectorEntity)
        {
            var task = UpdateAsync(connectorEntity);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Asynchronously updates a document in the collection.
        /// </summary>
        /// <param name="connectorEntity">The document to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public async Task<bool> UpdateAsync(ConnectorEntity connectorEntity)
        {
            await _arangoDbAccess.Client.Document.PutDocumentAsync(_collectionName, connectorEntity.Key, connectorEntity);
            return true;
        }

        /// <summary>
        /// Deletes a document by key.
        /// </summary>
        /// <param name="key">The key of the document to delete.</param>
        /// <returns>True if deletion was successful.</returns>
        public bool Delete(string key)
        {
            var task = DeleteAsync(key);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Asynchronously deletes a document by key.
        /// </summary>
        /// <param name="key">The key of the document to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates success.</returns>
        public async Task<bool> DeleteAsync(string key)
        {
            await _arangoDbAccess.Client.Document.DeleteDocumentAsync<ConnectorEntity>(_collectionName, key);
            return true;
        }

        /// <summary>
        /// Disposes the database access resources.
        /// </summary>
        public void Dispose()
        {
            _arangoDbAccess.Dispose();
        }
    }
}