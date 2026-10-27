// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;
using System;

namespace SharpConnector.Configuration
{
    /// <summary>
    /// Provides configuration settings for RavenDB.
    /// </summary>
    public class RavenDbConfig : IConnectorConfig
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }

        /// <summary>
        /// Optional path to the client certificate (.pfx) used to authenticate against a secured RavenDB server.
        /// </summary>
        public string CertificatePath { get; }

        /// <summary>
        /// Optional password protecting the client certificate. Used verbatim.
        /// </summary>
        public string CertificatePassword { get; }

        #region NOT USED
        public string CollectionName { get; private set; }
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

        public RavenDbConfig(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ConnectionString = configuration[AppConfigParameterEnums.connectionstring.ToString()]?.Trim();
            DatabaseName = configuration[AppConfigParameterEnums.databasename.ToString()]?.Trim();
            CertificatePath = configuration[AppConfigParameterEnums.certificatepath.ToString()]?.Trim();
            CertificatePassword = configuration[AppConfigParameterEnums.certificatepassword.ToString()];

            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new ArgumentException("RavenDB connection string is required.", nameof(configuration));
            if (!Uri.TryCreate(ConnectionString, UriKind.Absolute, out _))
                throw new ArgumentException($"RavenDB connection string '{ConnectionString}' is not a valid absolute URL.", nameof(configuration));
            if (string.IsNullOrWhiteSpace(DatabaseName))
                throw new ArgumentException("RavenDB database name is required.", nameof(configuration));
        }
    }
}
