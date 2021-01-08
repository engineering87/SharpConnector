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
        public virtual IConnectorConfig GetConfigurationStrategy(IConfigurationSection section, ConnectorTypeEnums connectorTypes)
        {
            return connectorTypes switch
            {
                ConnectorTypeEnums.Redis => new RedisConfig(section),
                ConnectorTypeEnums.MongoDb => new MongoDbConfig(section),
                ConnectorTypeEnums.LiteDb => new LiteDbConfig(section),
                ConnectorTypeEnums.Memcached => new MemcachedConfig(section),
                _ => section.Get<IConnectorConfig>()
            };
        }
    }
}
