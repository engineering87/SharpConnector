// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using SharpConnector.Configuration;
using System;

namespace SharpConnector.Connectors.DynamoDb
{
    public class DynamoDbAccess : IDisposable
    {
        private readonly Lazy<AmazonDynamoDBClient> _client;
        public AmazonDynamoDBClient GetClient() => _client.Value;

        /// <summary>
        /// Create a new DynamoDbAccess instance.
        /// </summary>
        /// <param name="connectorConfig">The DynamoDB configuration.</param>
        public DynamoDbAccess(DynamoDbConfig connectorConfig)
        {
            var awsCredentials = new BasicAWSCredentials(connectorConfig.AccessKey, connectorConfig.SecretKey);
            var config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(connectorConfig.Region),
                ServiceURL = connectorConfig.ServiceUrl,
                UseHttp = connectorConfig.UseHttp
            };

            _client = new Lazy<AmazonDynamoDBClient>(() => new AmazonDynamoDBClient(awsCredentials, config));
        }

        public void Dispose()
        {
            if (_client.IsValueCreated)
            {
                _client.Value.Dispose();
            }
        }
    }
}
