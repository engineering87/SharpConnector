// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using SharpConnector.Entities;
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
        public Task<ConnectorEntity> GetAsync(string key)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                var entity = session.Load<ConnectorEntity>(key);
                return Task.FromResult(entity);
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
        public Task<bool> InsertAsync(ConnectorEntity connectorEntity)
        {
            DeleteAsync(connectorEntity.Key);
            var insert = Insert(connectorEntity);
            return Task.FromResult(insert);
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
        public Task<bool> DeleteAsync(string key)
        {
            using (var session = _ravenDbAccess.Store.OpenSession())
            {
                session.Delete(key);
                session.SaveChanges();
            }
            return Task.FromResult(true);
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
        public Task<bool> UpdateAsync(ConnectorEntity connectorEntity)
        {
            var update = Update(connectorEntity);
            return Task.FromResult(update);
        }
    }
}
