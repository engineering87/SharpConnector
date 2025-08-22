// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Raven.Client.Documents;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.RavenDb
{
    public class RavenDbWrapper
    {
        private readonly RavenDbAccess _ravenDbAccess;

        /// <summary>
        /// Create a new RavenDbWrapper instance.
        /// </summary>
        /// <param name="ravenDbConfig">The RavenDB connector configuration.</param>
        public RavenDbWrapper(RavenDbConfig ravenDbConfig)
        {
            _ravenDbAccess = new RavenDbAccess(ravenDbConfig);
        }

        /// <summary>
        /// Retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public ConnectorEntity Get(string key)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                return session.Load<ConnectorEntity>(key);
            }
        }

        /// <summary>
        /// Asynchronously retrieve the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<ConnectorEntity> GetAsync(string key, CancellationToken ct = default)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                return await session.LoadAsync<ConnectorEntity>(key, ct).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieve all values.
        /// </summary>
        public List<ConnectorEntity> GetAll()
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                return session
                    .Query<ConnectorEntity>()
                    .Customize(cr => cr.WaitForNonStaleResults())
                    .ToList();
            }
        }

        /// <summary>
        /// Asynchronously retrieve all values.
        /// </summary>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task whose result contains the list of <see cref="ConnectorEntity"/>.</returns>
        public async Task<List<ConnectorEntity>> GetAllAsync(CancellationToken ct = default)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                return await session
                    .Query<ConnectorEntity>()
                    .Customize(cr => cr.WaitForNonStaleResults())
                    .ToListAsync(ct)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Set the key to hold the specified value.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        public bool Insert(ConnectorEntity connectorEntity)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                session.Store(connectorEntity, connectorEntity.Key);
                session.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Asynchronously set the key to hold the specified value.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> InsertAsync(ConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                await session.StoreAsync(connectorEntity, connectorEntity.Key, ct).ConfigureAwait(false);
                await session.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            return true;
        }

        /// <summary>
        /// Insert multiple entities.
        /// </summary>
        /// <param name="connectorEntities">The entities to store.</param>
        public bool InsertMany(List<ConnectorEntity> connectorEntities)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                foreach (var entity in connectorEntities)
                {
                    session.Store(entity, entity.Key);
                }
                session.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Asynchronously insert multiple entities.
        /// </summary>
        /// <param name="connectorEntities">The entities to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> InsertManyAsync(List<ConnectorEntity> connectorEntities, CancellationToken ct = default)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                foreach (var entity in connectorEntities)
                {
                    ct.ThrowIfCancellationRequested();
                    await session.StoreAsync(entity, entity.Key, ct).ConfigureAwait(false);
                }
                await session.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            return true;
        }

        /// <summary>
        /// Remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        public bool Delete(string key)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                var entity = session.Load<ConnectorEntity>(key);
                if (entity != null)
                {
                    session.Delete(key);
                    session.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Asynchronously remove the specified key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                var entity = await session.LoadAsync<ConnectorEntity>(key, ct).ConfigureAwait(false);
                if (entity != null)
                {
                    session.Delete(key);
                    await session.SaveChangesAsync(ct).ConfigureAwait(false);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Update the specified key.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        public bool Update(ConnectorEntity connectorEntity)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                var entity = session.Load<ConnectorEntity>(connectorEntity.Key);
                if (entity == null) return false;
                entity.Payload = connectorEntity.Payload;
                entity.Expiration = connectorEntity.Expiration;
                session.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Asynchronously update the specified key.
        /// </summary>
        /// <param name="connectorEntity">The entity to store.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        public async Task<bool> UpdateAsync(ConnectorEntity connectorEntity, CancellationToken ct = default)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                var entity = await session.LoadAsync<ConnectorEntity>(connectorEntity.Key, ct).ConfigureAwait(false);
                if (entity == null) return false;
                entity.Payload = connectorEntity.Payload;
                entity.Expiration = connectorEntity.Expiration;
                await session.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            return true;
        }

        /// <summary>
        /// Check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>True if the item exists; otherwise, false.</returns>
        public bool Exists(string key)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                var entity = session.Load<ConnectorEntity>(key);
                return entity != null;
            }
        }

        /// <summary>
        /// Asynchronously check whether an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the item exists; otherwise, false.</returns>
        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                var entity = await session.LoadAsync<ConnectorEntity>(key, ct).ConfigureAwait(false);
                return entity != null;
            }
        }

        /// <summary>
        /// Query the RavenDB database with a filter function.
        /// </summary>
        /// <param name="filter">A function to filter the results.</param>
        /// <returns>A collection of filtered <see cref="ConnectorEntity"/> instances.</returns>
        public IEnumerable<ConnectorEntity> Query(Func<ConnectorEntity, bool> filter)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                return session
                    .Query<ConnectorEntity>()
                    .Customize(cr => cr.WaitForNonStaleResults())
                    .Where(filter)
                    .ToList();
            }
        }

        /// <summary>
        /// Asynchronously query the RavenDB database with a filter function.
        /// </summary>
        /// <param name="filter">A function to filter the results.</param>
        /// <param name="ct">A token to cancel the asynchronous operation.</param>
        /// <returns>A task whose result is a collection of filtered <see cref="ConnectorEntity"/> instances.</returns>
        public async Task<IEnumerable<ConnectorEntity>> QueryAsync(Func<ConnectorEntity, bool> filter, CancellationToken ct = default)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                var allEntities = await session
                    .Query<ConnectorEntity>()
                    .Customize(cr => cr.WaitForNonStaleResults())
                    .ToListAsync(ct)
                    .ConfigureAwait(false);

                return allEntities.Where(filter);
            }
        }
    }
}