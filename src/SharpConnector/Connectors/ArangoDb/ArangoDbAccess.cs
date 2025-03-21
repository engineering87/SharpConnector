// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using ArangoDBNetStandard;
using ArangoDBNetStandard.Transport.Http;
using SharpConnector.Configuration;
using System;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.ArangoDb
{
    /// <summary>
    /// Manages access to the ArangoDB database.
    /// </summary>
    public class ArangoDbAccess : IDisposable
    {
        public ArangoDBClient Client { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArangoDbAccess"/> class.
        /// </summary>
        /// <param name="arangoDbConfig">The ArangoDb configuration.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public ArangoDbAccess(ArangoDbConfig arangoDbConfig)
        {
            if (string.IsNullOrWhiteSpace(arangoDbConfig.ConnectionString))
                throw new ArgumentNullException(nameof(arangoDbConfig.ConnectionString), "Connection string cannot be null or empty.");

            if (!Uri.TryCreate(arangoDbConfig.ConnectionString, UriKind.Absolute, out Uri baseUri))
                throw new UriFormatException("Invalid connection string format. Must be a valid URI.");

            var httpTransport = HttpApiTransport.UsingBasicAuth(
                baseUri,
                arangoDbConfig.Username,
                arangoDbConfig.Password
            );

            Client = new ArangoDBClient(httpTransport);
        }

        /// <summary>
        /// Checks if the connection to the database is active.
        /// </summary>
        /// <returns>True if the connection is active; otherwise, false.</returns>
        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                var databases = await Client.Database
                    .GetDatabasesAsync()
                    .ConfigureAwait(false);
                return !databases.Error;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Releases the resources used by the <see cref="ArangoDbAccess"/> class.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}