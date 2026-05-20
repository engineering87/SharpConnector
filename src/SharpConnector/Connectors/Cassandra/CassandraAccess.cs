// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Cassandra;
using SharpConnector.Configuration;
using System;
using System.Linq;

namespace SharpConnector.Connectors.Cassandra
{
    /// <summary>
    /// Manages access to an Apache Cassandra cluster and the associated session.
    /// </summary>
    public class CassandraAccess : IDisposable
    {
        public ICluster Cluster { get; }
        public ISession Session { get; }
        public string Keyspace { get; }
        public string TableName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CassandraAccess"/> class.
        /// </summary>
        /// <param name="cassandraConfig">The Cassandra configuration.</param>
        public CassandraAccess(CassandraConfig cassandraConfig)
        {
            if (cassandraConfig == null)
                throw new ArgumentNullException(nameof(cassandraConfig), "Cassandra configuration cannot be null.");

            Keyspace = cassandraConfig.DatabaseName;
            TableName = cassandraConfig.TableName;

            var (contactPoints, port) = ParseContactPoints(cassandraConfig.ConnectionString);

            var builder = global::Cassandra.Cluster.Builder()
                .AddContactPoints(contactPoints);

            if (port.HasValue)
                builder = builder.WithPort(port.Value);

            if (!string.IsNullOrEmpty(cassandraConfig.Username))
            {
                builder = builder.WithCredentials(
                    cassandraConfig.Username,
                    cassandraConfig.Password ?? string.Empty);
            }

            Cluster = builder.Build();

            // Ensure keyspace exists, then connect to it.
            using (var bootstrapSession = Cluster.Connect())
            {
                bootstrapSession.Execute(
                    $"CREATE KEYSPACE IF NOT EXISTS {Keyspace} " +
                    "WITH replication = {'class':'SimpleStrategy','replication_factor':1};");
            }

            Session = Cluster.Connect(Keyspace);

            // Ensure the storage table exists.
            Session.Execute(
                $"CREATE TABLE IF NOT EXISTS {TableName} (" +
                "key text PRIMARY KEY, " +
                "payload text, " +
                "expiration bigint);");
        }

        /// <summary>
        /// Parses the contact points string in the form "host1,host2:9042".
        /// </summary>
        private static (string[] hosts, int? port) ParseContactPoints(string connectionString)
        {
            var tokens = connectionString
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToArray();

            int? parsedPort = null;
            var hosts = tokens.Select(t =>
            {
                var idx = t.LastIndexOf(':');
                if (idx > 0 && int.TryParse(t.Substring(idx + 1), out var p))
                {
                    parsedPort = p;
                    return t.Substring(0, idx);
                }
                return t;
            }).ToArray();

            return (hosts, parsedPort);
        }

        /// <summary>
        /// Disposes the underlying Cassandra session and cluster.
        /// </summary>
        public void Dispose()
        {
            Session?.Dispose();
            Cluster?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
