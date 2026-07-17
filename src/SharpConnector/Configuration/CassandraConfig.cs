// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;
using System;

namespace SharpConnector.Configuration
{
    /// <summary>
    /// Provides configuration settings for Apache Cassandra.
    /// </summary>
    public class CassandraConfig : IConnectorConfig
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }
        public string TableName { get; }
        public string Username { get; }
        public string Password { get; }

        #region NOT USED
        public int DatabaseNumber { get; private set; }
        public string CollectionName { get; private set; }
        public string BucketName { get; private set; }
        public string AccessKey { get; private set; }
        public string SecretKey { get; private set; }
        public string Region { get; private set; }
        public string ServiceUrl { get; private set; }
        public bool UseHttp { get; private set; }
        #endregion

        public CassandraConfig(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ConnectionString = configuration[AppConfigParameterEnums.connectionstring.ToString()]?.Trim();
            if (string.IsNullOrEmpty(ConnectionString))
                throw new ArgumentException("Cassandra ConnectionString (contact points) is required but was not found in configuration.");

            DatabaseName = configuration[AppConfigParameterEnums.databasename.ToString()]?.Trim();
            if (string.IsNullOrEmpty(DatabaseName))
                throw new ArgumentException("Cassandra DatabaseName (keyspace) is required but was not found in configuration.");

            TableName = configuration[AppConfigParameterEnums.tablename.ToString()]?.Trim();
            if (string.IsNullOrEmpty(TableName))
                throw new ArgumentException("Cassandra TableName is required but was not found in configuration.");

            Username = configuration[AppConfigParameterEnums.username.ToString()]?.Trim();
            Password = configuration[AppConfigParameterEnums.password.ToString()]?.Trim();
        }
    }
}
