// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpConnector.Configuration;
using SharpConnector.Connectors.Redis;
using SharpConnector.Connectors.RavenDb;
using SharpConnector.Middleware;
using SharpConnector.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharpConnector.Tests
{
    [TestClass]
    public class RemainingFixesTests
    {
        private static IConfiguration BuildConfiguration(Dictionary<string, string> values) =>
            new ConfigurationBuilder().AddInMemoryCollection(values).Build();

        #region RavenDB

        [TestMethod]
        public void RavenDbConfig_MissingUrl_Throws() =>
            Assert.ThrowsExactly<ArgumentException>(() => new RavenDbConfig(BuildConfiguration(new() { ["databasename"] = "db" })));

        [TestMethod]
        public void RavenDbConfig_InvalidUrl_Throws() =>
            Assert.ThrowsExactly<ArgumentException>(() => new RavenDbConfig(BuildConfiguration(new() { ["connectionstring"] = "not a url", ["databasename"] = "db" })));

        [TestMethod]
        public void RavenDbConfig_MissingDatabase_Throws() =>
            Assert.ThrowsExactly<ArgumentException>(() => new RavenDbConfig(BuildConfiguration(new() { ["connectionstring"] = "http://localhost:8080" })));

        [TestMethod]
        public void RavenDbConfig_ReadsCertificateSettings()
        {
            var config = new RavenDbConfig(BuildConfiguration(new()
            {
                ["connectionstring"] = "https://localhost:8080",
                ["databasename"] = "db",
                ["certificatepath"] = " cert.pfx ",
                ["certificatepassword"] = " p@ss "
            }));

            Assert.AreEqual("cert.pfx", config.CertificatePath);
            Assert.AreEqual(" p@ss ", config.CertificatePassword);
        }

        [TestMethod]
        public void RavenDbAccess_LoadCertificate_NoPath_ReturnsNull()
        {
            var config = new RavenDbConfig(BuildConfiguration(new() { ["connectionstring"] = "http://localhost:8080", ["databasename"] = "db" }));
            Assert.IsNull(RavenDbAccess.LoadCertificate(config));
        }

        [TestMethod]
        public void RavenDbAccess_LoadCertificate_MissingFile_Throws()
        {
            var config = new RavenDbConfig(BuildConfiguration(new()
            {
                ["connectionstring"] = "https://localhost:8080",
                ["databasename"] = "db",
                ["certificatepath"] = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".pfx")
            }));
            Assert.ThrowsExactly<FileNotFoundException>(() => RavenDbAccess.LoadCertificate(config));
        }

        #endregion

        #region Redis

        [TestMethod]
        public void RedisConfig_KeyPrefix_DefaultsToEmpty()
        {
            var config = new RedisConfig(BuildConfiguration(new() { ["connectionstring"] = "localhost:6379" }));
            Assert.AreEqual(string.Empty, config.KeyPrefix);
        }

        [TestMethod]
        public void RedisConfig_KeyPrefix_IsRead()
        {
            var config = new RedisConfig(BuildConfiguration(new() { ["connectionstring"] = "localhost:6379", ["keyprefix"] = " app: " }));
            Assert.AreEqual("app:", config.KeyPrefix);
        }

        [TestMethod]
        public void RedisWrapper_BuildScanPattern()
        {
            Assert.IsTrue(RedisWrapper.BuildScanPattern(string.Empty).IsNull);
            Assert.AreEqual("app:*", (string)RedisWrapper.BuildScanPattern("app:"));
        }

        #endregion

        #region DynamoDB

        [TestMethod]
        public void DynamoDbConfig_MissingTable_Throws() =>
            Assert.ThrowsExactly<ArgumentException>(() => new DynamoDbConfig(BuildConfiguration(new() { ["region"] = "eu-west-1" })));

        [TestMethod]
        public void DynamoDbConfig_MissingRegionAndServiceUrl_Throws() =>
            Assert.ThrowsExactly<ArgumentException>(() => new DynamoDbConfig(BuildConfiguration(new() { ["tablename"] = "t" })));

        [TestMethod]
        public void DynamoDbConfig_InvalidServiceUrl_Throws() =>
            Assert.ThrowsExactly<ArgumentException>(() => new DynamoDbConfig(BuildConfiguration(new() { ["tablename"] = "t", ["serviceurl"] = "bad url" })));

        [TestMethod]
        public void DynamoDbConfig_Valid_DoesNotThrow()
        {
            var config = new DynamoDbConfig(BuildConfiguration(new() { ["tablename"] = "t", ["serviceurl"] = "http://localhost:8000" }));
            Assert.AreEqual("t", config.TableName);
        }

        #endregion

        #region Misc

        [TestMethod]
        public void AddSharpConnectorServices_DoesNotRegisterMvc()
        {
            var services = new ServiceCollection();
            services.AddSharpConnectorServices<string>();

            Assert.IsFalse(services.Any(d => d.ServiceType.FullName?.StartsWith("Microsoft.AspNetCore.Mvc") == true));
        }

        [TestMethod]
        public void AsyncHelper_RunSync_ReturnsResult() =>
            Assert.AreEqual(42, AsyncHelper.RunSync(async () => { await Task.Yield(); return 42; }));

        [TestMethod]
        public void AsyncHelper_RunSync_PropagatesException() =>
            Assert.ThrowsExactly<InvalidOperationException>(() => AsyncHelper.RunSync(() => Task.FromException(new InvalidOperationException())));

        #endregion
    }
}
