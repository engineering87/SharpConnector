using Enyim.Caching;
using Enyim.Caching.Configuration;
using Microsoft.Extensions.Logging;
using SharpConnector.Entities;
using System.Collections.Generic;
using Enyim.Caching.Memcached;
using SharpConnector.Configuration;

namespace SharpConnector.Connectors.Memcached
{
    public class MemcachedAccess
    {
        public readonly IMemcachedClient<ConnectorEntity> MemcachedClient;

        /// <summary>
        /// Create a new MemcachedAccess instance.
        /// </summary>
        /// <param name="memcachedConfig">The Memcached configuration.</param>
        public MemcachedAccess(MemcachedConfig memcachedConfig)
        {
            var loggerFactory = new LoggerFactory();
            var splittedConnstring = memcachedConfig.ConnectionString.Split(":");
            var options = new MemcachedClientOptions()
            {
                Protocol = MemcachedProtocol.Text,
                Servers = new List<Server> { 
                    new Server
                    {
                        Address = splittedConnstring[0].Trim(),
                        Port = int.Parse(splittedConnstring[1].Trim())
                    }
                }
            };
            var config = new MemcachedClientConfiguration(loggerFactory, options);
            MemcachedClient = new MemcachedClient<ConnectorEntity>(loggerFactory, config);
        }
    }
}
