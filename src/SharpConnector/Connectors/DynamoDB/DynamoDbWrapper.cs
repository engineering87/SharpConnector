// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.DynamoDb
{
    public class DynamoDbWrapper
    {
        private readonly DynamoDbAccess _dynamoDbAccess;
        private readonly Table _table;

        /// <summary>
        /// Create a new DynamoDbWrapper instance.
        /// </summary>
        /// <param name="dynamoDbConfig">The DynamoDB connector configuration.</param>
        public DynamoDbWrapper(DynamoDbConfig dynamoDbConfig)
        {
            _dynamoDbAccess = new DynamoDbAccess(dynamoDbConfig);

            var client = _dynamoDbAccess.GetClient();
            
            _table = new TableBuilder(client, dynamoDbConfig.TableName)
                .AddHashKey("Key", DynamoDBEntryType.String)
                .Build();
        }

        /// <summary>
        /// Get an item by key (sync).
        /// </summary>
        /// <param name="key">The key of the item.</param>
        public ConnectorEntity Get(string key)
        {
            var document = _table.GetItemAsync(key).GetAwaiter().GetResult();
            return document != null
                ? JsonConvert.DeserializeObject<ConnectorEntity>(document.ToJson())
                : null;
        }

        /// <summary>
        /// Get an item by key (async).
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<ConnectorEntity> GetAsync(string key, CancellationToken ct = default)
        {
            var document = await _table.GetItemAsync(key, ct).ConfigureAwait(false);
            return document != null
                ? JsonConvert.DeserializeObject<ConnectorEntity>(document.ToJson())
                : null;
        }

        /// <summary>
        /// Get all items in the table (sync).
        /// </summary>
        public IEnumerable<ConnectorEntity> GetAll()
        {
            var scanConfig = new ScanOperationConfig();
            var search = _table.Scan(scanConfig);

            var result = new List<ConnectorEntity>();
            do
            {
                var documents = search.GetNextSetAsync().GetAwaiter().GetResult();
                result.AddRange(documents.Select(doc =>
                    JsonConvert.DeserializeObject<ConnectorEntity>(doc.ToJson())));
            } while (!search.IsDone);

            return result;
        }

        /// <summary>
        /// Get all items in the table (async).
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<IEnumerable<ConnectorEntity>> GetAllAsync(CancellationToken ct = default)
        {
            var scanConfig = new ScanOperationConfig();
            var search = _table.Scan(scanConfig);

            var result = new List<ConnectorEntity>();
            do
            {
                ct.ThrowIfCancellationRequested();
                var documents = await search.GetNextSetAsync(ct).ConfigureAwait(false);
                result.AddRange(documents.Select(doc =>
                    JsonConvert.DeserializeObject<ConnectorEntity>(doc.ToJson())));
            } while (!search.IsDone);

            return result;
        }

        /// <summary>
        /// Insert (Put) an item (sync).
        /// </summary>
        /// <param name="connectorEntity">The entity to insert.</param>
        public bool Insert(ConnectorEntity connectorEntity)
        {
            var document = Document.FromJson(JsonConvert.SerializeObject(connectorEntity));
            _table.PutItemAsync(document).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Insert (Put) an item (async).
        /// </summary>
        /// <param name="connectorEntity">The entity to insert.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> InsertAsync(ConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            var document = Document.FromJson(JsonConvert.SerializeObject(connectorEntity));
            await _table.PutItemAsync(document, ct).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Insert multiple items (sync).
        /// </summary>
        /// <param name="connectorEntities">The entities to insert.</param>
        public bool InsertMany(List<ConnectorEntity> connectorEntities)
        {
            var batchWrite = _table.CreateBatchWrite();
            foreach (var entity in connectorEntities)
            {
                var document = Document.FromJson(JsonConvert.SerializeObject(entity));
                batchWrite.AddDocumentToPut(document);
            }

            batchWrite.ExecuteAsync().GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Insert multiple items (async).
        /// </summary>
        /// <param name="connectorEntities">The entities to insert.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities, CancellationToken ct = default)
        {
            var batchWrite = _table.CreateBatchWrite();
            foreach (var entity in connectorEntities)
            {
                ct.ThrowIfCancellationRequested();
                var document = Document.FromJson(JsonConvert.SerializeObject(entity));
                batchWrite.AddDocumentToPut(document);
            }

            await batchWrite.ExecuteAsync(ct).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Insert multiple items (async) from an enumerable.
        /// </summary>
        /// <param name="values">Entities to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the batch completed successfully.</returns>
        public async Task<bool> InsertManyAsync(IEnumerable<ConnectorEntity> values, CancellationToken ct = default)
        {
            var batchWrite = _table.CreateBatchWrite();

            foreach (var entity in values)
            {
                ct.ThrowIfCancellationRequested();
                var document = Document.FromJson(JsonConvert.SerializeObject(entity));
                batchWrite.AddDocumentToPut(document);
            }

            await batchWrite.ExecuteAsync(ct).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Delete an item by key (sync).
        /// </summary>
        /// <param name="key">The key of the item to delete.</param>
        public bool Delete(string key)
        {
            _table.DeleteItemAsync(key).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Delete an item by key (async).
        /// </summary>
        /// <param name="key">The key of the item to delete.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            await _table.DeleteItemAsync(key, ct).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Update an existing item (sync).
        /// </summary>
        /// <param name="connectorEntity">The entity to update.</param>
        public bool Update(ConnectorEntity connectorEntity)
        {
            // DynamoDB Put is an upsert; reuse Insert.
            return Insert(connectorEntity);
        }

        /// <summary>
        /// Update an existing item (async).
        /// </summary>
        /// <param name="connectorEntity">The entity to update.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public Task<bool> UpdateAsync(ConnectorEntity connectorEntity, CancellationToken ct = default) =>
            InsertAsync(connectorEntity, ct);

        /// <summary>
        /// Check whether a key exists (sync).
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public bool Exists(string key)
        {
            var document = _table.GetItemAsync(key).GetAwaiter().GetResult();
            return document != null;
        }

        /// <summary>
        /// Check whether a key exists (async).
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            var document = await _table.GetItemAsync(key, ct).ConfigureAwait(false);
            return document != null;
        }

        /// <summary>
        /// Execute a filtered query.
        /// </summary>
        public List<ConnectorEntity> Query(Func<ConnectorEntity, bool> filter)
        {
            if (filter is null) throw new ArgumentNullException(nameof(filter), "Filter cannot be null.");

            var all = GetAll();
            return (all ?? [])
                .Where(filter)
                .ToList();
        }

        /// <summary>
        /// Asynchronously execute a filtered query (not supported in this wrapper).
        /// </summary>
        public Task<List<ConnectorEntity>> QueryAsync(Func<ConnectorEntity, bool> filter, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            ct.ThrowIfCancellationRequested();

            var list = Query(filter);
            return Task.FromResult(list);
        }
    }
}