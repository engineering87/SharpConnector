// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;
using System;

namespace SharpConnector.Configuration
{
    /// <summary>
    /// Provides configuration settings for MongoDb.
    /// </summary>
    public class MongoDbConfig : IConnectorConfig
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }

        #region NOT USED
        public int DatabaseNumber { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string BucketName { get; private set; }
        public string AccessKey { get; private set; }
        public string SecretKey { get; private set; }
        public string Region { get; private set; }
        public string ServiceUrl { get; private set; }
        public bool UseHttp { get; private set; }
        public string TableName { get; private set; }
        #endregion

        public MongoDbConfig(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ConnectionString = configuration[AppConfigParameterEnums.connectionstring.ToString()]?.Trim();
            DatabaseName = configuration[AppConfigParameterEnums.databasename.ToString()]?.Trim();
            CollectionName = configuration[AppConfigParameterEnums.collectionname.ToString()]?.Trim();
        }
    }
}