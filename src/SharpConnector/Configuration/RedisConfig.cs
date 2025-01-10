// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;
using System;

namespace SharpConnector.Configuration
{
    /// <summary>
    /// Provides configuration settings for Redis.
    /// </summary>
    public class RedisConfig : IConnectorConfig
    {
        public string ConnectionString { get; }
        public int DatabaseNumber { get; } // 0 default

        #region NOT USED
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }
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

        public RedisConfig(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ConnectionString = configuration[AppConfigParameterEnums.connectionstring.ToString()]?.Trim();
            int.TryParse(configuration[AppConfigParameterEnums.databasenumber.ToString()], out var dbNumber);
            DatabaseNumber = dbNumber;
        }
    }
}
