// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;
using System;

namespace SharpConnector.Configuration
{
    /// <summary>
    /// Provides configuration settings for DynamoDb.
    /// </summary>
    public class DynamoDbConfig : IConnectorConfig
    {
        public string AccessKey { get; }
        public string SecretKey { get; }
        public string Region { get; }
        public string ServiceUrl { get; }
        public bool UseHttp { get; }
        public string TableName { get; }

        #region NOT USED
        public string ConnectionString { get; private set; }
        public int DatabaseNumber { get; private set; }
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string BucketName { get; private set; }
        #endregion

        public DynamoDbConfig(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            AccessKey = configuration[AppConfigParameterEnums.accesskey.ToString()]?.Trim();
            SecretKey = configuration[AppConfigParameterEnums.secretkey.ToString()]?.Trim();
            Region = configuration[AppConfigParameterEnums.region.ToString()]?.Trim();
            ServiceUrl = configuration[AppConfigParameterEnums.serviceurl.ToString()]?.Trim();
            bool.TryParse(configuration[AppConfigParameterEnums.usehttp.ToString()], out var useHttp);
            UseHttp = useHttp;
            TableName = configuration[AppConfigParameterEnums.tablename.ToString()]?.Trim();

            if (string.IsNullOrWhiteSpace(TableName))
                throw new ArgumentException("DynamoDB table name is required.", nameof(configuration));
            if (string.IsNullOrWhiteSpace(Region) && string.IsNullOrWhiteSpace(ServiceUrl))
                throw new ArgumentException("DynamoDB requires either a region or a service URL.", nameof(configuration));
            if (!string.IsNullOrWhiteSpace(ServiceUrl) && !Uri.TryCreate(ServiceUrl, UriKind.Absolute, out _))
                throw new ArgumentException($"DynamoDB service URL '{ServiceUrl}' is not a valid absolute URL.", nameof(configuration));
        }
    }
}
