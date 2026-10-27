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

            try
            {
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
            catch
            {
                Session?.Dispose();
                Cluster.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Parses the contact points string in the form "host1,host2:9042,[::1]:9042".
        /// Bare IPv6 addresses (e.g. "fe80::1") are supported; a port must be specified using brackets.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the string is empty, a contact point or port is invalid, or hosts declare different ports.</exception>
        internal static (string[] hosts, int? port) ParseContactPoints(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Cassandra connection string cannot be null or empty.", nameof(connectionString));

            var tokens = connectionString
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            int? parsedPort = null;
            var hosts = new string[tokens.Length];
            for (var i = 0; i < tokens.Length; i++)
            {
                var (host, port) = ParseHost(tokens[i]);
                hosts[i] = host;
                if (!port.HasValue)
                    continue;
                if (parsedPort.HasValue && parsedPort.Value != port.Value)
                    throw new ArgumentException("All Cassandra contact points must use the same port.", nameof(connectionString));
                parsedPort = port;
            }

            return (hosts, parsedPort);
        }

        private static (string host, int? port) ParseHost(string token)
        {
            // [IPv6]:port or [IPv6]
            if (token.StartsWith('['))
            {
                var end = token.IndexOf(']');
                if (end < 0)
                    throw new ArgumentException($"Invalid contact point '{token}'.");
                var host = token[1..end];
                var rest = token[(end + 1)..];
                if (rest.Length == 0)
                    return (host, null);
                if (rest[0] == ':' && int.TryParse(rest[1..], out var p6))
                    return (host, p6);
                throw new ArgumentException($"Invalid contact point '{token}'.");
            }

            // More than one ':' without brackets: bare IPv6 address, no port.
            var first = token.IndexOf(':');
            if (first < 0 || first != token.LastIndexOf(':'))
                return (token, null);

            if (int.TryParse(token[(first + 1)..], out var p))
                return (token[..first], p);

            throw new ArgumentException($"Invalid port in contact point '{token}'.");
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
