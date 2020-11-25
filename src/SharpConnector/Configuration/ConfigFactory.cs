// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Operations;

namespace SharpConnector.Configuration
{
    public class ConfigFactory
    {
        /// <summary>
        /// Get a new ConnectorConfig instance related to the connector type and config.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="connectorTypes"></param>
        /// <returns></returns>
        public ConnectorConfig GetConfigurationStrategy(IConfigurationSection section, ConnectorTypes connectorTypes)
        {
            return connectorTypes switch
            {
                ConnectorTypes.Redis => section.Get<RedisConfig>(),
                ConnectorTypes.MongoDb => section.Get<MongoDbConfig>(),
                _ => section.Get<ConnectorConfig>()
            };
        }
    }
}
