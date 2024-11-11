// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;

namespace SharpConnector.Operations
{
    /// <summary>
    /// The Operation factory abstract class.
    /// </summary>
    public abstract class OperationFactory
    {
        /// <summary>
        /// Get the configuration strategy for the connector type.
        /// </summary>
        /// <param name="section">The configuration section.</param>
        /// <param name="connectorTypes">The connector type.</param>
        /// <returns></returns>
        public virtual IConnectorConfig GetConfigurationStrategy(IConfigurationSection section, ConnectorTypeEnums connectorTypes)
        {
            return connectorTypes switch
            {
                ConnectorTypeEnums.Redis => new RedisConfig(section),
                ConnectorTypeEnums.MongoDb => new MongoDbConfig(section),
                ConnectorTypeEnums.LiteDb => new LiteDbConfig(section),
                ConnectorTypeEnums.Memcached => new MemcachedConfig(section),
                ConnectorTypeEnums.RavenDb => new RavenDbConfig(section),
                ConnectorTypeEnums.Couchbase => new CouchbaseConfig(section),
                _ => section.Get<IConnectorConfig>()
            };
        }
    }
}
