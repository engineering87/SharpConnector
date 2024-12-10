// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
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

        private readonly Dictionary<ConnectorTypeEnums, Func<object, IOperations<T>>> _strategies;

        public OperationsFactory(IConfigurationSection section)
        {
            _section = section ?? throw new ArgumentNullException(nameof(section));

            _strategies = new Dictionary<ConnectorTypeEnums, Func<object, IOperations<T>>>
            {
                { ConnectorTypeEnums.Redis, config => new RedisOperations<T>((RedisConfig)config) },
                { ConnectorTypeEnums.MongoDb, config => new MongoDbOperations<T>((MongoDbConfig)config) },
                { ConnectorTypeEnums.LiteDb, config => new LiteDbOperations<T>((LiteDbConfig)config) },
                { ConnectorTypeEnums.Memcached, config => new MemcachedOperations<T>((MemcachedConfig)config) },
                { ConnectorTypeEnums.RavenDb, config => new RavenDbOperations<T>((RavenDbConfig)config) },
                { ConnectorTypeEnums.Couchbase, config => new CouchbaseOperations<T>((CouchbaseConfig)config) },
                { ConnectorTypeEnums.DynamoDb, config => new DynamoDbOperations<T>((DynamoDbConfig)config) }
            };
        }

        /// <summary>
        /// Get a new Operations instance related to the connector type and config.
        /// </summary>
        /// <returns>Instance of IOperations&lt;T&gt;.</returns>
        public IOperations<T> GetStrategy()
        {
            var dbType = _section
                .GetChildren()
                .FirstOrDefault(s => s.Key.Equals("instance", StringComparison.OrdinalIgnoreCase))?.Value;

            if (!Enum.TryParse(dbType, true, out ConnectorTypeEnums connectorType))
                throw new InvalidOperationException("Instance section for SharpConnector was not found.");

            var connectorConfig = GetConfigurationStrategy(_section, connectorType);

            if (!_strategies.TryGetValue(connectorType, out var strategy))
                throw new ArgumentOutOfRangeException(nameof(connectorType), "Unsupported connector type.");

            return strategy(connectorConfig);
        }
    }
}
