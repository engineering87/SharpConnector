// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpConnector.Configuration;
using System;
using System.Collections.Generic;

namespace SharpConnector.Tests
{
    [TestClass]
    public class CassandraConfigTests
    {
        private static IConfigurationSection BuildSection(Dictionary<string, string> values)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();

            return configuration.GetSection("ConnectorConfig");
        }

        [TestMethod]
        public void Ctor_ValidConfiguration_PopulatesAllProperties()
        {
            var section = BuildSection(new Dictionary<string, string>
            {
                ["ConnectorConfig:connectionstring"] = "127.0.0.1:9042",
                ["ConnectorConfig:databasename"] = "sharpconnector",
                ["ConnectorConfig:tablename"] = "items",
                ["ConnectorConfig:username"] = "cassandra",
                ["ConnectorConfig:password"] = "cassandra"
            });

            var config = new CassandraConfig(section);

            Assert.AreEqual("127.0.0.1:9042", config.ConnectionString);
            Assert.AreEqual("sharpconnector", config.DatabaseName);
            Assert.AreEqual("items", config.TableName);
            Assert.AreEqual("cassandra", config.Username);
            Assert.AreEqual("cassandra", config.Password);
        }

        [TestMethod]
        public void Ctor_TrimsWhitespace()
        {
            var section = BuildSection(new Dictionary<string, string>
            {
                ["ConnectorConfig:connectionstring"] = "  127.0.0.1:9042  ",
                ["ConnectorConfig:databasename"] = "  ks  ",
                ["ConnectorConfig:tablename"] = "  tbl  ",
                ["ConnectorConfig:username"] = "  user  ",
                ["ConnectorConfig:password"] = "  pwd  "
            });

            var config = new CassandraConfig(section);

            Assert.AreEqual("127.0.0.1:9042", config.ConnectionString);
            Assert.AreEqual("ks", config.DatabaseName);
            Assert.AreEqual("tbl", config.TableName);
            Assert.AreEqual("user", config.Username);
            Assert.AreEqual("pwd", config.Password);
        }

        [TestMethod]
        public void Ctor_OptionalCredentials_AreAllowedToBeNull()
        {
            var section = BuildSection(new Dictionary<string, string>
            {
                ["ConnectorConfig:connectionstring"] = "127.0.0.1:9042",
                ["ConnectorConfig:databasename"] = "sharpconnector",
                ["ConnectorConfig:tablename"] = "items"
            });

            var config = new CassandraConfig(section);

            Assert.IsNull(config.Username);
            Assert.IsNull(config.Password);
        }

        [TestMethod]
        public void Ctor_NullConfiguration_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new CassandraConfig(null));
        }

        [TestMethod]
        public void Ctor_MissingConnectionString_Throws()
        {
            var section = BuildSection(new Dictionary<string, string>
            {
                ["ConnectorConfig:databasename"] = "sharpconnector",
                ["ConnectorConfig:tablename"] = "items"
            });

            Assert.ThrowsExactly<ArgumentException>(() => new CassandraConfig(section));
        }

        [TestMethod]
        public void Ctor_MissingDatabaseName_Throws()
        {
            var section = BuildSection(new Dictionary<string, string>
            {
                ["ConnectorConfig:connectionstring"] = "127.0.0.1:9042",
                ["ConnectorConfig:tablename"] = "items"
            });

            Assert.ThrowsExactly<ArgumentException>(() => new CassandraConfig(section));
        }

        [TestMethod]
        public void Ctor_MissingTableName_Throws()
        {
            var section = BuildSection(new Dictionary<string, string>
            {
                ["ConnectorConfig:connectionstring"] = "127.0.0.1:9042",
                ["ConnectorConfig:databasename"] = "sharpconnector"
            });

            Assert.ThrowsExactly<ArgumentException>(() => new CassandraConfig(section));
        }
    }
}
