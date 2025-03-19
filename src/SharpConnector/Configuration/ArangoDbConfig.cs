// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;
using System;

namespace SharpConnector.Configuration
{
    public class ArangoDbConfig : IConnectorConfig
    {
        public string ConnectionString { get; }
        public string CollectionName { get; }
        public string Username { get; }
        public string Password { get; }

        #region NOT USED
        public string DatabaseName { get; private set; }
        public int DatabaseNumber { get; private set; }
        public string BucketName { get; private set; }
        public string AccessKey { get; private set; }
        public string SecretKey { get; private set; }
        public string Region { get; private set; }
        public string ServiceUrl { get; private set; }
        public bool UseHttp { get; private set; }
        public string TableName { get; private set; }
        #endregion

        public ArangoDbConfig(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ConnectionString = configuration[AppConfigParameterEnums.connectionstring.ToString()]?.Trim();
            CollectionName = configuration[AppConfigParameterEnums.collectionname.ToString()]?.Trim();
            Username = configuration[AppConfigParameterEnums.username.ToString()]?.Trim();
            Password = configuration[AppConfigParameterEnums.password.ToString()]?.Trim();
        }
    }
}
