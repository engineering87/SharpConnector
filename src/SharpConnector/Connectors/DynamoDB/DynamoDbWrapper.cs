// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="dynamoDbConfig">The DynamoDB connector config.</param>
        public DynamoDbWrapper(DynamoDbConfig dynamoDbConfig)
        {
            _dynamoDbAccess = new DynamoDbAccess(dynamoDbConfig);
            _table = Table.LoadTable(_dynamoDbAccess.GetClient(), dynamoDbConfig.TableName);
        }

        /// <summary>
        /// Get an item by key synchronously.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <returns></returns>
        public ConnectorEntity Get(string key)
        {
            var document = _table.GetItemAsync(key).GetAwaiter().GetResult();
            return document != null
                ? JsonConvert.DeserializeObject<ConnectorEntity>(document.ToJson())
                : null;
        }

        /// <summary>
        /// Get an item by key.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <returns></returns>
        public async Task<ConnectorEntity> GetAsync(string key)
        {
            var document = await _table.GetItemAsync(key).ConfigureAwait(false);
            return document != null
                ? JsonConvert.DeserializeObject<ConnectorEntity>(document.ToJson())
                : null;
        }

        /// <summary>
        /// Get all items in the table synchronously.
        /// </summary>
        /// <returns></returns>
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
        /// Get all items in the table.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ConnectorEntity>> GetAllAsync()
        {
            var scanConfig = new ScanOperationConfig();
            var search = _table.Scan(scanConfig);

            var result = new List<ConnectorEntity>();
            do
            {
                var documents = await search.GetNextSetAsync().ConfigureAwait(false);
                result.AddRange(documents.Select(doc =>
                    JsonConvert.DeserializeObject<ConnectorEntity>(doc.ToJson())));
            } while (!search.IsDone);

            return result;
        }

        /// <summary>
        /// Insert an item synchronously.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to insert.</param>
        /// <returns></returns>
        public bool Insert(ConnectorEntity connectorEntity)
        {
            var document = Document.FromJson(JsonConvert.SerializeObject(connectorEntity));
            _table.PutItemAsync(document).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Insert an item.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to insert.</param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(ConnectorEntity connectorEntity)
        {
            var document = Document.FromJson(JsonConvert.SerializeObject(connectorEntity));
            await _table.PutItemAsync(document).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Insert multiple items synchronously.
        /// </summary>
        /// <param name="connectorEntities">The list of ConnectorEntity objects to insert.</param>
        /// <returns></returns>
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
        /// Insert multiple items.
        /// </summary>
        /// <param name="connectorEntities">The list of ConnectorEntity objects to insert.</param>
        /// <returns></returns>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities)
        {
            var batchWrite = _table.CreateBatchWrite();
            foreach (var entity in connectorEntities)
            {
                var document = Document.FromJson(JsonConvert.SerializeObject(entity));
                batchWrite.AddDocumentToPut(document);
            }

            await batchWrite.ExecuteAsync().ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Insert multiple items asynchronously.
        /// </summary>
        /// <param name="values">A collection of values to store.</param>
        /// <returns>A task representing the asynchronous operation, with the result being true if all insertions are successful.</returns>
        public async Task<bool> InsertManyAsync(IEnumerable<ConnectorEntity> values)
        {
            var batchWrite = _table.CreateBatchWrite();

            foreach (var value in values)
            {
                var connectorEntity = new ConnectorEntity
                {
                    Key = value.Key,
                    Payload = value,
                    Expiration = value.Expiration
                };

                var document = Document.FromJson(JsonConvert.SerializeObject(connectorEntity));
                batchWrite.AddDocumentToPut(document);
            }

            await batchWrite.ExecuteAsync().ConfigureAwait(false);
            return true;
        }


        /// <summary>
        /// Delete an item by key synchronously.
        /// </summary>
        /// <param name="key">The key of the item to delete.</param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            _table.DeleteItemAsync(key).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Delete an item by key.
        /// </summary>
        /// <param name="key">The key of the item to delete.</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string key)
        {
            await _table.DeleteItemAsync(key).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Update an existing item synchronously.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to update.</param>
        /// <returns></returns>
        public bool Update(ConnectorEntity connectorEntity)
        {
            return Insert(connectorEntity); // Reusing Insert for update as DynamoDB "Put" operation also performs an update if the item exists.
        }

        /// <summary>
        /// Update an existing item.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to update.</param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(ConnectorEntity connectorEntity) =>
            InsertAsync(connectorEntity);

        /// <summary>
        /// Checks if a Key exists.
        /// </summary>
        public bool Exists(string key)
        {
            var document = _table
                .GetItemAsync(key)
                .GetAwaiter()
                .GetResult();
            return document != null;
        }

        /// <summary>
        /// Checks if a Key exists asynchronously.
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            var document = await _table
                .GetItemAsync(key)
                .ConfigureAwait(false);
            return document != null;
        }
    }
}
