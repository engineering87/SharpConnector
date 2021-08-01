// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SharpConnector.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;

namespace SharpConnector.Operations
{
    public class OperationsFactory<T> : OperationFactory
    {
        private readonly IConfigurationSection _section;

        public OperationsFactory(IConfigurationSection section)
        {
            this._section = section;
        }

        /// <summary>
        /// Get a new Operations instance related to the connector type and config.
        /// </summary>
        /// <returns></returns>
        public IOperations<T> GetStrategy()
        {
            var dbType = _section.GetChildren().FirstOrDefault(s => s.Key.ToLower() == "instance")?.Value;

            if (!Enum.TryParse(dbType, true, out ConnectorTypeEnums connectorTypes))
                throw new Exception("Instance section for SharpConnector was not found.");

            var connectorConfig = GetConfigurationStrategy(_section, connectorTypes);
            switch (connectorTypes)
            {
                case ConnectorTypeEnums.Redis:
                    {
                        var redisConfig = connectorConfig as RedisConfig;
                        return new RedisOperations<T>(redisConfig);
                    }
                case ConnectorTypeEnums.MongoDb:
                    {
                        var mongoDbConfig = connectorConfig as MongoDbConfig;
                        return new MongoDbOperations<T>(mongoDbConfig);
                    }
                case ConnectorTypeEnums.LiteDb:
                    {
                        var liteDbConfig = connectorConfig as LiteDbConfig;
                        return new LiteDbOperations<T>(liteDbConfig);
                    }
                case ConnectorTypeEnums.Memcached:
                    {
                        var memcachedConfig = connectorConfig as MemcachedConfig;
                        return new MemcachedOperations<T>(memcachedConfig);
                    }
                case ConnectorTypeEnums.RavenDb:
                    {
                        var ravenDbConfig = connectorConfig as RavenDbConfig;
                        return new RavenDbOperations<T>(ravenDbConfig);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
