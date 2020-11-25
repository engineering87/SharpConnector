// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using SharpConnector.Configuration;

namespace SharpConnector.Operations
{
    public class OperationsFactory<T>
    {
        /// <summary>
        /// Get a new Operations instance related to the connector type and config.
        /// </summary>
        /// <param name="connectorTypes">The connector type.</param>
        /// <param name="connectorConfig">The connector config.</param>
        /// <returns></returns>
        public Operations<T> GetOperationsStrategy(ConnectorTypes connectorTypes, ConnectorConfig connectorConfig)
        {
            switch (connectorTypes)
            {
                case ConnectorTypes.Redis:
                {
                    var redisConfig = connectorConfig as RedisConfig;
                    return new RedisOperations<T>(redisConfig);
                }
                case ConnectorTypes.MongoDb:
                {
                    var mongoDbConfig = connectorConfig as MongoDbConfig;
                    return new MongoDbOperations<T>(mongoDbConfig);
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
