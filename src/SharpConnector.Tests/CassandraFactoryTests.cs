// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpConnector.Configuration;
using SharpConnector.Enums;
using SharpConnector.Operations;
using System;
using System.Collections.Generic;

namespace SharpConnector.Tests
{
    [TestClass]
    public class CassandraFactoryTests
    {
        private static IConfigurationSection BuildSection(Dictionary<string, string> values)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();

            return configuration.GetSection("ConnectorConfig");
        }

        [TestMethod]
        public void ConnectorTypeEnum_ContainsCassandra()
        {
            var defined = Enum.IsDefined(typeof(ConnectorTypeEnums), ConnectorTypeEnums.Cassandra);
            Assert.IsTrue(defined);
        }

        [TestMethod]
        public void OperationFactory_GetConfigurationStrategy_ReturnsCassandraConfig()
        {
            var section = BuildSection(new Dictionary<string, string>
            {
                ["ConnectorConfig:connectionstring"] = "127.0.0.1:9042",
                ["ConnectorConfig:databasename"] = "sharpconnector",
                ["ConnectorConfig:tablename"] = "items"
            });

            var factory = new OperationsFactory<string>(section);
            var config = factory.GetConfigurationStrategy(section, ConnectorTypeEnums.Cassandra);

            Assert.IsNotNull(config);
            Assert.IsInstanceOfType(config, typeof(CassandraConfig));
            Assert.AreEqual("sharpconnector", config.DatabaseName);
            Assert.AreEqual("items", config.TableName);
        }
    }
}
