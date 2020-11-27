// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Configuration;
using StackExchange.Redis;
using System;

namespace SharpConnector.Connectors.Redis
{
    internal class RedisAccess : IDisposable
    {
        private readonly Lazy<ConnectionMultiplexer> _connection;
        public ConnectionMultiplexer GetConnection() => _connection.Value;

        /// <summary>
        /// Create a new RedisAccess instance.
        /// </summary>
        /// <param name="connectorConfig"></param>
        public RedisAccess(ConnectorConfig connectorConfig)
        {
            var conn = ConfigurationOptions.Parse(connectorConfig.ConnectionString);
            conn.AllowAdmin = true;
            _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(conn));
        }

        public void Dispose()
        {
            _connection?.Value?.Close();
            _connection?.Value?.Dispose();
        }
    }
}
