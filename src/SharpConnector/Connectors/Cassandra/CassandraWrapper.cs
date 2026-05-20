// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Cassandra;
using Newtonsoft.Json;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.Cassandra
{
    /// <summary>
    /// Provides CRUD operations over an Apache Cassandra table using a key/payload schema.
    /// </summary>
    public class CassandraWrapper : IDisposable
    {
        private readonly CassandraAccess _cassandraAccess;
        private readonly PreparedStatement _getStatement;
        private readonly PreparedStatement _getAllStatement;
        private readonly PreparedStatement _insertStatement;
        private readonly PreparedStatement _insertStatementWithTtl;
        private readonly PreparedStatement _updateStatement;
        private readonly PreparedStatement _deleteStatement;
        private readonly PreparedStatement _existsStatement;

        /// <summary>
        /// Initializes a new instance of the <see cref="CassandraWrapper"/> class.
        /// </summary>
        /// <param name="cassandraConfig">The Cassandra configuration.</param>
        public CassandraWrapper(CassandraConfig cassandraConfig)
        {
            _cassandraAccess = new CassandraAccess(cassandraConfig);

            var table = _cassandraAccess.TableName;
            _getStatement = _cassandraAccess.Session.Prepare(
                $"SELECT key, payload, expiration FROM {table} WHERE key = ?;");
            _getAllStatement = _cassandraAccess.Session.Prepare(
                $"SELECT key, payload, expiration FROM {table};");
            _insertStatement = _cassandraAccess.Session.Prepare(
                $"INSERT INTO {table} (key, payload, expiration) VALUES (?, ?, ?);");
            _insertStatementWithTtl = _cassandraAccess.Session.Prepare(
                $"INSERT INTO {table} (key, payload, expiration) VALUES (?, ?, ?) USING TTL ?;");
            _updateStatement = _cassandraAccess.Session.Prepare(
                $"UPDATE {table} SET payload = ?, expiration = ? WHERE key = ?;");
            _deleteStatement = _cassandraAccess.Session.Prepare(
                $"DELETE FROM {table} WHERE key = ?;");
            _existsStatement = _cassandraAccess.Session.Prepare(
                $"SELECT key FROM {table} WHERE key = ?;");
        }

        /// <summary>
        /// Disposes the underlying Cassandra access.
        /// </summary>
        public void Dispose()
        {
            _cassandraAccess?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public ConnectorEntity Get(string key)
        {
            var row = _cassandraAccess.Session
                .Execute(_getStatement.Bind(key))
                .FirstOrDefault();
            return MapRow(row);
        }

        /// <summary>
        /// Asynchronously retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<ConnectorEntity> GetAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var rowSet = await _cassandraAccess.Session
                .ExecuteAsync(_getStatement.Bind(key))
                .ConfigureAwait(false);
            return MapRow(rowSet.FirstOrDefault());
        }

        /// <summary>
        /// Retrieve all values.
        /// </summary>
        public List<ConnectorEntity> GetAll()
        {
            return _cassandraAccess.Session
                .Execute(_getAllStatement.Bind())
                .Select(MapRow)
                .Where(e => e != null)
                .ToList();
        }

        /// <summary>
        /// Asynchronously retrieve all values.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<List<ConnectorEntity>> GetAllAsync(CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var rowSet = await _cassandraAccess.Session
                .ExecuteAsync(_getAllStatement.Bind())
                .ConfigureAwait(false);
            return rowSet
                .Select(MapRow)
                .Where(e => e != null)
                .ToList();
        }

        /// <summary>
        /// Insert or replace the specified entity (Cassandra INSERT is an upsert).
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        public bool Insert(ConnectorEntity connectorEntity)
        {
            var statement = BuildInsertStatement(connectorEntity);
            _cassandraAccess.Session.Execute(statement);
            return true;
        }

        /// <summary>
        /// Asynchronously insert or replace the specified entity.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> InsertAsync(ConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var statement = BuildInsertStatement(connectorEntity);
            await _cassandraAccess.Session
                .ExecuteAsync(statement)
                .ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Insert multiple entities using a logged batch.
        /// </summary>
        /// <param name="connectorEntities">The entities to store.</param>
        public bool InsertMany(List<ConnectorEntity> connectorEntities)
        {
            if (connectorEntities == null || connectorEntities.Count == 0)
                return true;

            var batch = new BatchStatement();
            foreach (var entity in connectorEntities)
                batch.Add(BuildInsertStatement(entity));

            _cassandraAccess.Session.Execute(batch);
            return true;
        }

        /// <summary>
        /// Asynchronously insert multiple entities using a logged batch.
        /// </summary>
        /// <param name="connectorEntities">The entities to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities, CancellationToken ct = default)
        {
            if (connectorEntities == null || connectorEntities.Count == 0)
                return true;

            ct.ThrowIfCancellationRequested();

            var batch = new BatchStatement();
            foreach (var entity in connectorEntities)
                batch.Add(BuildInsertStatement(entity));

            await _cassandraAccess.Session
                .ExecuteAsync(batch)
                .ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Update an existing entity.
        /// </summary>
        /// <param name="connectorEntity">The entity to update.</param>
        public bool Update(ConnectorEntity connectorEntity)
        {
            var payload = SerializePayload(connectorEntity.Payload);
            var expirationTicks = connectorEntity.Expiration?.Ticks ?? 0L;

            _cassandraAccess.Session.Execute(
                _updateStatement.Bind(payload, expirationTicks, connectorEntity.Key));
            return true;
        }

        /// <summary>
        /// Asynchronously update an existing entity.
        /// </summary>
        /// <param name="connectorEntity">The entity to update.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> UpdateAsync(ConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var payload = SerializePayload(connectorEntity.Payload);
            var expirationTicks = connectorEntity.Expiration?.Ticks ?? 0L;

            await _cassandraAccess.Session
                .ExecuteAsync(_updateStatement.Bind(payload, expirationTicks, connectorEntity.Key))
                .ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public bool Delete(string key)
        {
            _cassandraAccess.Session.Execute(_deleteStatement.Bind(key));
            return true;
        }

        /// <summary>
        /// Asynchronously remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await _cassandraAccess.Session
                .ExecuteAsync(_deleteStatement.Bind(key))
                .ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        public bool Exists(string key)
        {
            return _cassandraAccess.Session
                .Execute(_existsStatement.Bind(key))
                .Any();
        }

        /// <summary>
        /// Asynchronously check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var rowSet = await _cassandraAccess.Session
                .ExecuteAsync(_existsStatement.Bind(key))
                .ConfigureAwait(false);
            return rowSet.Any();
        }

        /// <summary>
        /// Query items by filtering in memory.
        /// </summary>
        /// <param name="filter">Predicate used to filter items.</param>
        public List<ConnectorEntity> Query(Func<ConnectorEntity, bool> filter)
        {
            return GetAll().Where(filter).ToList();
        }

        /// <summary>
        /// Asynchronously query items by filtering in memory.
        /// </summary>
        /// <param name="filter">Predicate used to filter items.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<List<ConnectorEntity>> QueryAsync(Func<ConnectorEntity, bool> filter, CancellationToken ct = default)
        {
            var entities = await GetAllAsync(ct).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
            return entities.Where(filter).ToList();
        }

        private BoundStatement BuildInsertStatement(ConnectorEntity entity)
        {
            var payload = SerializePayload(entity.Payload);
            var expirationTicks = entity.Expiration?.Ticks ?? 0L;

            if (entity.Expiration.HasValue && entity.Expiration.Value > TimeSpan.Zero)
            {
                var ttlSeconds = (int)Math.Max(1, Math.Min(int.MaxValue, entity.Expiration.Value.TotalSeconds));
                return _insertStatementWithTtl.Bind(entity.Key, payload, expirationTicks, ttlSeconds);
            }

            return _insertStatement.Bind(entity.Key, payload, expirationTicks);
        }

        private static string SerializePayload(object payload)
        {
            return payload == null ? null : JsonConvert.SerializeObject(payload);
        }

        private static ConnectorEntity MapRow(Row row)
        {
            if (row == null)
                return null;

            var key = row.GetValue<string>("key");
            var payloadJson = row.GetValue<string>("payload");
            var expirationTicks = row.GetValue<long>("expiration");

            object payload = null;
            if (!string.IsNullOrEmpty(payloadJson))
                payload = JsonConvert.DeserializeObject(payloadJson);

            TimeSpan? expiration = expirationTicks > 0
                ? TimeSpan.FromTicks(expirationTicks)
                : (TimeSpan?)null;

            return new ConnectorEntity(key, payload ?? string.Empty, expiration);
        }
    }
}
