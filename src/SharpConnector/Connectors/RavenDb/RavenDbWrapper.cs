// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.RavenDb
{
    public class RavenDbWrapper
    {
        private readonly RavenDbAccess _ravenDbAccess;

        /// <summary>
        /// Create a new RavenDbWrapper instance.
        /// </summary>
        /// <param name="ravenDbConfig">The RavenDb connector config.</param>
        public RavenDbWrapper(RavenDbConfig ravenDbConfig)
        {
            _ravenDbAccess = new RavenDbAccess(ravenDbConfig);
        }

        public ConnectorEntity Get(string key)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                return session.Load<ConnectorEntity>(key);
            }
        }

        /// <summary>
        /// Get the value of Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public async Task<ConnectorEntity> GetAsync(string key)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                return await session.LoadAsync<ConnectorEntity>(key);
            }
        }

        /// <summary>
        /// Get all the values.
        /// </summary>
        /// <returns></returns>
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
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
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
        /// Set the Key to hold the value.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(ConnectorEntity connectorEntity)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                await session.StoreAsync(connectorEntity, connectorEntity.Key);
                await session.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// Multiple set operation.
        /// </summary>
        /// <param name="connectorEntities">The ConnectorEntities to store.</param>
        /// <returns></returns>
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
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                session.Delete(key);
                session.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Removes the specified Key.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string key)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                session.Delete(key);
                await session.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public bool Update(ConnectorEntity connectorEntity)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                var entity = session.Load<ConnectorEntity>(connectorEntity.Key);
                entity.Payload = connectorEntity.Payload;
                entity.Expiration = connectorEntity.Expiration;
                session.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="connectorEntity">The ConnectorEntity to store.</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(ConnectorEntity connectorEntity)
        {
            using (var session = _ravenDbAccess.Store.OpenAsyncSession())
            {
                var entity = await session.LoadAsync<ConnectorEntity>(connectorEntity.Key);
                entity.Payload = connectorEntity.Payload;
                entity.Expiration = connectorEntity.Expiration;
                await session.SaveChangesAsync();
            }
            return true;
        }
    }
}
