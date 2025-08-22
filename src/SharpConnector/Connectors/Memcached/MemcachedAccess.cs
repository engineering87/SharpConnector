// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Microsoft.Extensions.Logging;
using SharpConnector.Configuration;
using System;
using System.Collections.Generic;

namespace SharpConnector.Connectors.Memcached
{
    public class MemcachedAccess : IDisposable
    {
        public readonly IMemcachedClient MemcachedClient;
        private readonly LoggerFactory _loggerFactory;

        /// <summary>
        /// Create a new MemcachedAccess instance.
        /// </summary>
        /// <param name="memcachedConfig">The Memcached configuration.</param>
        public MemcachedAccess(MemcachedConfig memcachedConfig)
        {
            _loggerFactory = new LoggerFactory();

            var servers = new List<Server>();
            foreach (var part in memcachedConfig.ConnectionString.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmed = part.Trim();
                var idx = trimmed.LastIndexOf(':');
                if (idx <= 0 || idx == trimmed.Length - 1)
                    throw new FormatException($"Invalid Memcached connection string segment: '{trimmed}' (expected host:port)");

                var host = trimmed[..idx].Trim();
                if (!int.TryParse(trimmed[(idx + 1)..], out var port))
                    throw new FormatException($"Invalid port in connection string segment: '{trimmed}'");

                servers.Add(new Server { Address = host, Port = port });
            }

            var options = new MemcachedClientOptions
            {
                Protocol = MemcachedProtocol.Text,
                Servers = servers
            };

            var config = new MemcachedClientConfiguration(_loggerFactory, options);
            MemcachedClient = new MemcachedClient(_loggerFactory, config);
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