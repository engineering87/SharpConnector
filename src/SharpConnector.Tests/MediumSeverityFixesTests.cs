// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using SharpConnector.Configuration;
using SharpConnector.Connectors.Cassandra;
using SharpConnector.Connectors.LiteDb;
using SharpConnector.Connectors.Memcached;
using SharpConnector.Connectors.MongoDb;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpConnector.Tests
{
    [TestClass]
    public class MediumSeverityFixesTests
    {
        #region Cassandra

        [TestMethod]
        public void Cassandra_ParseContactPoints_HostsWithSamePort()
        {
            var (hosts, port) = CassandraAccess.ParseContactPoints("h1:9042, h2:9042");

            CollectionAssert.AreEqual(new[] { "h1", "h2" }, hosts);
            Assert.AreEqual(9042, port);
        }

        [TestMethod]
        public void Cassandra_ParseContactPoints_BracketedIPv6WithPort()
        {
            var (hosts, port) = CassandraAccess.ParseContactPoints("[::1]:9043");

            CollectionAssert.AreEqual(new[] { "::1" }, hosts);
            Assert.AreEqual(9043, port);
        }

        [TestMethod]
        public void Cassandra_ParseContactPoints_BareIPv6_NoPort()
        {
            var (hosts, port) = CassandraAccess.ParseContactPoints("fe80::1");

            CollectionAssert.AreEqual(new[] { "fe80::1" }, hosts);
            Assert.IsNull(port);
        }

        [TestMethod]
        public void Cassandra_ParseContactPoints_DifferentPorts_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CassandraAccess.ParseContactPoints("h1:9042,h2:9043"));
        }

        [TestMethod]
        public void Cassandra_ParseContactPoints_InvalidPort_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CassandraAccess.ParseContactPoints("h1:abc"));
        }

        [TestMethod]
        public void Cassandra_ParseContactPoints_Empty_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CassandraAccess.ParseContactPoints("  "));
        }

        [TestMethod]
        public void Cassandra_ToTtlSeconds_HandlesEdgeCases()
        {
            Assert.AreEqual(0, CassandraWrapper.ToTtlSeconds(null));
            Assert.AreEqual(0, CassandraWrapper.ToTtlSeconds(TimeSpan.Zero));
            Assert.AreEqual(0, CassandraWrapper.ToTtlSeconds(TimeSpan.FromSeconds(-5)));
            Assert.AreEqual(1, CassandraWrapper.ToTtlSeconds(TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(60, CassandraWrapper.ToTtlSeconds(TimeSpan.FromMinutes(1)));
        }

        #endregion

        #region MongoDB

        [TestMethod]
        public void MongoDb_BuildUpsertRequests_CreatesOneUpsertPerEntity()
        {
            var entities = new[]
            {
                new MongoConnectorEntity("a", "1", null),
                new MongoConnectorEntity("b", "2", null)
            };

            var requests = MongoDbWrapper.BuildUpsertRequests(entities);

            Assert.HasCount(2, requests);
            Assert.IsTrue(requests.All(r => r is ReplaceOneModel<MongoConnectorEntity> { IsUpsert: true }));
        }

        #endregion

        #region LiteDB

        private static LiteDbWrapper CreateLiteDbWrapper() =>
            new(new LiteDbConfig(BuildConfiguration(new() { ["connectionstring"] = ":memory:" })));

        [TestMethod]
        public void LiteDb_InsertSameKeyTwice_DoesNotCreateDuplicates()
        {
            using var wrapper = CreateLiteDbWrapper();

            Assert.IsTrue(wrapper.Insert(new LiteDbConnectorEntity("k", "v1", null)));
            Assert.IsTrue(wrapper.Insert(new LiteDbConnectorEntity("k", "v2", null)));

            Assert.HasCount(1, wrapper.GetAll());
            Assert.AreEqual("v2", wrapper.Get("k").Payload);
        }

        #endregion

        #region Memcached

        [TestMethod]
        public void Memcached_ParseServers_ValidConnectionString()
        {
            var servers = MemcachedAccess.ParseServers("host1:11211, host2:11212");

            Assert.HasCount(2, servers);
            Assert.AreEqual("host1", servers[0].Address);
            Assert.AreEqual(11212, servers[1].Port);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" , ")]
        public void Memcached_ParseServers_EmptyConnectionString_Throws(string connectionString)
        {
            Assert.ThrowsExactly<ArgumentException>(() => MemcachedAccess.ParseServers(connectionString));
        }

        [TestMethod]
        [DataRow("host")]
        [DataRow("host:abc")]
        [DataRow(":11211")]
        public void Memcached_ParseServers_InvalidSegment_Throws(string connectionString)
        {
            Assert.ThrowsExactly<FormatException>(() => MemcachedAccess.ParseServers(connectionString));
        }

        #endregion

        #region Configuration

        [TestMethod]
        public void Configs_DoNotTrimPassword()
        {
            var configuration = BuildConfiguration(new()
            {
                ["ConnectorConfig:connectionString"] = "localhost",
                ["ConnectorConfig:username"] = " user ",
                ["ConnectorConfig:password"] = " p@ss "
            });
            var section = configuration.GetSection("ConnectorConfig");

            Assert.AreEqual(" p@ss ", new CouchbaseConfig(section).Password);
            Assert.AreEqual(" p@ss ", new ArangoDbConfig(section).Password);
        }

        #endregion

        #region Converter

        [TestMethod]
        public void Converter_NullValue_Throws()
        {
            var values = new Dictionary<string, string> { ["a"] = "1", ["b"] = null };

            Assert.ThrowsExactly<ArgumentException>(() => values.ToConnectorEntityList());
            Assert.ThrowsExactly<ArgumentException>(() => values.ToLiteDbConnectorEntityList());
            Assert.ThrowsExactly<ArgumentException>(() => values.ToMongoDbConnectorEntityList());
        }

        [TestMethod]
        public void Converter_EmptyKey_Throws()
        {
            var values = new Dictionary<string, string> { [""] = "1" };

            Assert.ThrowsExactly<ArgumentException>(() => values.ToConnectorEntityList());
        }

        #endregion

        private static IConfiguration BuildConfiguration(Dictionary<string, string> values) =>
            new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }
}
