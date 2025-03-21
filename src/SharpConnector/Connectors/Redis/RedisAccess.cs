// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using StackExchange.Redis;
using System;

namespace SharpConnector.Connectors.Redis
{
    public class RedisAccess : IDisposable
    {
        private readonly Lazy<ConnectionMultiplexer> _connection;
        public ConnectionMultiplexer Connection => _connection.Value;

        /// <summary>
        /// Create a new RedisAccess instance.
        /// </summary>
        /// <param name="connectorConfig">The Redis configuration.</param>
        public RedisAccess(RedisConfig connectorConfig)
        {
            if (connectorConfig == null)
            {
                throw new ArgumentNullException(nameof(connectorConfig));
            }

            var connOptions = ConfigurationOptions.Parse(connectorConfig.ConnectionString);
            connOptions.AllowAdmin = true;
            connOptions.ConnectRetry = 5;
            connOptions.ConnectTimeout = 5000;
            connOptions.KeepAlive = 10;

            _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connOptions));
        }

        /// <summary>
        /// Check if the connection is active.
        /// </summary>
        public bool IsConnected => Connection?.IsConnected ?? false;

        public void Dispose()
        {
            if (_connection.IsValueCreated)
            {
                _connection.Value.Close();
                _connection.Value.Dispose();
            }
        }
    }
}
