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
    public class OperationsFactory<T> : AOperationFactory
    {
        private readonly IConfigurationSection section;

        public OperationsFactory(IConfigurationSection section)
        {
            this.section = section;
        }

        /// <summary>
        /// Get a new Operations instance related to the connector type and config.
        /// </summary>
        /// <param name="connectorConfig">The connector config.</param>
        /// <returns></returns>
        public IOperations<T> GetStrategy()
        {
            var dbType = section.GetChildren().FirstOrDefault(s => s.Key == "Instance")?.Value;

            if (!Enum.TryParse(dbType, true, out ConnectorTypeEnums connectorTypes))
                throw new Exception("Instance not found");

            var connectorConfig = GetConfigurationStrategy(section, connectorTypes);
            IOperations<T> strategy = null;
            switch (connectorTypes)
            {
                case ConnectorTypeEnums.Redis:
                    {
                        var redisConfig = connectorConfig as RedisConfig;
                        strategy = new RedisOperations<T>(redisConfig);
                        break;
                    }
                case ConnectorTypeEnums.MongoDb:
                    {
                        var mongoDbConfig = connectorConfig as MongoDbConfig;
                        strategy = new MongoDbOperations<T>(mongoDbConfig);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return strategy;
        }

       
    }
}
