// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Microsoft.Extensions.Logging;
using SharpConnector.Entities;
using Enyim.Caching.Memcached;
using SharpConnector.Configuration;
using System;

namespace SharpConnector.Connectors.Memcached
{
    public class MemcachedAccess : IDisposable
    {
        public readonly IMemcachedClient<ConnectorEntity> MemcachedClient;
        private readonly LoggerFactory _loggerFactory;

        /// <summary>
        /// Create a new MemcachedAccess instance.
        /// </summary>
        /// <param name="memcachedConfig">The Memcached configuration.</param>
        public MemcachedAccess(MemcachedConfig memcachedConfig)
        {
            _loggerFactory = new LoggerFactory();
            var splittedConnstring = memcachedConfig.ConnectionString.Split(":");
            var options = new MemcachedClientOptions()
            {
                Protocol = MemcachedProtocol.Text,
                Servers = [ 
                    new Server
                    {
                        Address = splittedConnstring[0].Trim(),
                        Port = int.Parse(splittedConnstring[1].Trim())
                    }
                ]
            };
            var config = new MemcachedClientConfiguration(_loggerFactory, options);
            MemcachedClient = new MemcachedClient<ConnectorEntity>(_loggerFactory, config);
        }

        /// <summary>
        /// Dispose the MemcachedClient.
        /// </summary>
        public void Dispose()
        {
            MemcachedClient?.Dispose();
            _loggerFactory?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
