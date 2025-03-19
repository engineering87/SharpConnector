using System;
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;

namespace SharpConnector.Configuration
{
    /// <summary>
    /// Provides configuration settings for Memcached.
    /// </summary>
    public class MemcachedConfig : IConnectorConfig
    {
        public string ConnectionString { get; }

        #region NOT USED
        public int DatabaseNumber { get; private set; }
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

        public MemcachedConfig(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ConnectionString = configuration[AppConfigParameterEnums.connectionstring.ToString()]?.Trim();
        }
    }
}
