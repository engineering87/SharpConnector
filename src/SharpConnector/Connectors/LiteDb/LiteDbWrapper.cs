// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using System.Collections.Generic;
using System.Threading;
using System;

namespace SharpConnector.Connectors.LiteDb
{
    public class LiteDbWrapper
    {
        private readonly LiteDbAccess _liteDbAccess;

        /// <summary>
        /// Create a new LiteDbWrapper instance.
        /// </summary>
        /// <param name="liteDbConfig">The LiteDB connector configuration.</param>
        public LiteDbWrapper(LiteDbConfig liteDbConfig)
        {
            _liteDbAccess = new LiteDbAccess(liteDbConfig);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public LiteDbConnectorEntity Get(string key)
        {
            return _liteDbAccess.Collection
                .Find(LiteDB.Query.EQ("Key", new BsonValue(key)))
                .FirstOrDefault();
        }

        /// <summary>
        /// Asynchronously retrieve the value of the specified key.
        /// </summary>
        /// <remarks>LiteDB is synchronous; this method wraps the sync call.</remarks>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the operation.</param>
        public Task<LiteDbConnectorEntity> GetAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var entity = Get(key);
            return Task.FromResult(entity);
        }

        /// <summary>
        /// Retrieve all values.
        /// </summary>
        public IEnumerable<LiteDbConnectorEntity> GetAll()
        {
            return _liteDbAccess.Collection.Find(_ => true).ToList();
        }

        /// <summary>
        /// Asynchronously retrieve all values.
        /// </summary>
        /// <remarks>LiteDB is synchronous; this method wraps the sync call.</remarks>
        /// <param name="ct">A token to cancel the operation.</param>
        public Task<IEnumerable<LiteDbConnectorEntity>> GetAllAsync(CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var entities = GetAll();
            return Task.FromResult(entities);
        }

        /// <summary>
        /// Insert (or upsert if configured) the specified entity.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        public bool Insert(LiteDbConnectorEntity connectorEntity)
        {
            var result = _liteDbAccess.Collection.Insert(connectorEntity);
            return !result.IsNull;
        }

        /// <summary>
        /// Asynchronously insert (or upsert if configured) the specified entity.
        /// </summary>
        /// <remarks>LiteDB is synchronous; this method wraps the sync call.</remarks>
        /// <param name="connectorEntity">The entity to store.</param>
        /// <param name="ct">A token to cancel the operation.</param>
        public Task<bool> InsertAsync(LiteDbConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var result = Insert(connectorEntity);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Insert multiple entities in bulk.
        /// </summary>
        /// <param name="connectorEntities">Entities to store.</param>
        public bool InsertMany(List<LiteDbConnectorEntity> connectorEntities)
        {
            var insertedCount = _liteDbAccess.Collection.InsertBulk(connectorEntities);
            return insertedCount == connectorEntities?.Count;
        }

        /// <summary>
        /// Asynchronously insert multiple entities in bulk.
        /// </summary>
        /// <remarks>LiteDB is synchronous; this method wraps the sync call.</remarks>
        /// <param name="connectorEntities">Entities to store.</param>
        /// <param name="ct">A token to cancel the operation.</param>
        public Task<bool> InsertManyAsync(List<LiteDbConnectorEntity> connectorEntities, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var ok = InsertMany(connectorEntities);
            return Task.FromResult(ok);
        }

        /// <summary>
        /// Remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public bool Delete(string key)
        {
            return _liteDbAccess.Collection.DeleteMany(LiteDB.Query.EQ("Key", new BsonValue(key))) > 0;
        }

        /// <summary>
        /// Asynchronously remove the specified key.
        /// </summary>
        /// <remarks>LiteDB is synchronous; this method wraps the sync call.</remarks>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the operation.</param>
        public Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var deleted = Delete(key);
            return Task.FromResult(deleted);
        }

        /// <summary>
        /// Update the specified entity.
        /// </summary>
        /// <param name="connectorEntity">The entity to update.</param>
        public bool Update(LiteDbConnectorEntity connectorEntity)
        {
            return _liteDbAccess.Collection.Update(connectorEntity);
        }

        /// <summary>
        /// Asynchronously update the specified entity.
        /// </summary>
        /// <remarks>LiteDB is synchronous; this method wraps the sync call.</remarks>
        /// <param name="connectorEntity">The entity to update.</param>
        /// <param name="ct">A token to cancel the operation.</param>
        public Task<bool> UpdateAsync(LiteDbConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var result = Update(connectorEntity);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Check whether a key exists.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public bool Exists(string key)
        {
            return _liteDbAccess.Collection.Exists(LiteDB.Query.EQ("Key", new BsonValue(key)));
        }

        /// <summary>
        /// Asynchronously check whether a key exists.
        /// </summary>
        /// <remarks>LiteDB is synchronous; this method wraps the sync call.</remarks>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the operation.</param>
        public Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var exists = Exists(key);
            return Task.FromResult(exists);
        }

        /// <summary>
        /// Execute a filtered query (not supported by this wrapper).
        /// </summary>
        public List<ConnectorEntity> Query(Func<ConnectorEntity, bool> filter)
        {
            return _liteDbAccess.Collection
                .FindAll()
                .Cast<ConnectorEntity>()
                .Where(filter)
                .ToList();
        }

        /// <summary>
        /// Asynchronously execute a filtered query (not supported by this wrapper).
        /// </summary>
        /// <param name="filter">Filter predicate.</param>
        /// <param name="ct">A token to cancel the operation.</param>
        public Task<List<ConnectorEntity>> QueryAsync(Func<ConnectorEntity, bool> filter, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var list = Query(filter);
            return Task.FromResult(list);
        }
    }
}