// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpConnector.Configuration;
using SharpConnector.Connectors.LiteDb;
using SharpConnector.Connectors.Redis;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpConnector.Tests
{
    [TestClass]
    public class HighSeverityFixesTests
    {
        #region Expiration

        [TestMethod]
        public void ConnectorEntity_WithExpiration_SetsExpiresAt()
        {
            var before = DateTime.UtcNow;
            var entity = new ConnectorEntity("k", "v", TimeSpan.FromMinutes(5));

            Assert.IsNotNull(entity.ExpiresAt);
            Assert.IsTrue(entity.ExpiresAt.Value >= before.AddMinutes(5));
            Assert.IsFalse(entity.IsExpired());
        }

        [TestMethod]
        public void ConnectorEntity_WithoutExpiration_NeverExpires()
        {
            var entity = new ConnectorEntity("k", "v");

            Assert.IsNull(entity.ExpiresAt);
            Assert.IsFalse(entity.IsExpired());
        }

        [TestMethod]
        public void IsExpired_PastExpiresAt_ReturnsTrue()
        {
            var entity = new ConnectorEntity("k", "v") { ExpiresAt = DateTime.UtcNow.AddSeconds(-1) };

            Assert.IsTrue(entity.IsExpired());
            Assert.IsNull(entity.DiscardIfExpired());
        }

        [TestMethod]
        public void WhereNotExpired_FiltersExpiredAndNull()
        {
            var valid = new ConnectorEntity("a", "v", TimeSpan.FromMinutes(1));
            var expired = new ConnectorEntity("b", "v") { ExpiresAt = DateTime.UtcNow.AddMinutes(-1) };
            var list = new List<ConnectorEntity> { valid, expired, null };

            var result = list.WhereNotExpired().ToList();

            Assert.HasCount(1, result);
            Assert.AreEqual("a", result[0].Key);
        }

        [TestMethod]
        public void ExpiresAt_SurvivesJsonRoundTrip()
        {
            var entity = new ConnectorEntity("k", "v") { ExpiresAt = DateTime.UtcNow.AddSeconds(-5) };

            var copy = JsonConvert.DeserializeObject<ConnectorEntity>(JsonConvert.SerializeObject(entity));

            Assert.IsTrue(copy.IsExpired());
        }

        [TestMethod]
        public void RemainingTimeToLive_NoExpiration_ReturnsNull()
        {
            Assert.IsNull(new ConnectorEntity("k", "v").RemainingTimeToLive());
        }

        [TestMethod]
        public void RemainingTimeToLive_WithExpiration_ReturnsPositiveTtl()
        {
            var ttl = new ConnectorEntity("k", "v", TimeSpan.FromMinutes(10)).RemainingTimeToLive();

            Assert.IsNotNull(ttl);
            Assert.IsTrue(ttl.Value > TimeSpan.FromMinutes(9) && ttl.Value <= TimeSpan.FromMinutes(10));
        }

        [TestMethod]
        public void RemainingTimeToLive_AlreadyExpired_ReturnsMinimalPositiveTtl()
        {
            var entity = new ConnectorEntity("k", "v") { ExpiresAt = DateTime.UtcNow.AddMinutes(-1) };

            Assert.IsTrue(entity.RemainingTimeToLive() > TimeSpan.Zero);
        }

        #endregion

        #region Redis

        [TestMethod]
        public void Redis_IsWrongType_DetectsWrongTypeError()
        {
            var ex = new RedisServerException("WRONGTYPE Operation against a key holding the wrong kind of value");

            Assert.IsTrue(RedisWrapper.IsWrongType(ex));
        }

        [TestMethod]
        public void Redis_IsWrongType_IgnoresOtherErrors()
        {
            Assert.IsFalse(RedisWrapper.IsWrongType(new RedisServerException("ERR something else")));
        }

        [TestMethod]
        public void Redis_TryDeserialize_InvalidPayload_ReturnsNull()
        {
            Assert.IsNull(RedisWrapper.TryDeserialize("not-json"));
        }

        [TestMethod]
        public void Redis_TryDeserialize_ValidPayload_ReturnsEntity()
        {
            var json = JsonConvert.SerializeObject(new ConnectorEntity("k", "v"));

            Assert.AreEqual("k", RedisWrapper.TryDeserialize(json).Key);
        }

        [TestMethod]
        public void RedisConfig_ReadsDatabaseNumber()
        {
            var config = new RedisConfig(BuildConfiguration(new()
            {
                ["connectionstring"] = "localhost:6379",
                ["databasenumber"] = "3"
            }));

            Assert.AreEqual(3, config.DatabaseNumber);
        }

        #endregion

        #region ArangoDB

        [TestMethod]
        public void ArangoConnectorEntity_SerializesKeyAsDocumentKey()
        {
            var entity = new ArangoConnectorEntity(new ConnectorEntity("my-key", "v"));

            var json = JObject.Parse(JsonConvert.SerializeObject(entity));

            Assert.AreEqual("my-key", json["_key"]?.ToString());
            Assert.AreEqual("my-key", json["Key"]?.ToString());
        }

        [TestMethod]
        public void ArangoConnectorEntity_CopiesExpiration()
        {
            var source = new ConnectorEntity("k", "v", TimeSpan.FromMinutes(1));

            var entity = new ArangoConnectorEntity(source);

            Assert.AreEqual(source.Expiration, entity.Expiration);
            Assert.AreEqual(source.ExpiresAt, entity.ExpiresAt);
        }

        #endregion

        #region LiteDB (in-memory)

        private static LiteDbWrapper CreateLiteDbWrapper() =>
            new(new LiteDbConfig(BuildConfiguration(new() { ["connectionstring"] = ":memory:" })));

        [TestMethod]
        public void LiteDb_Update_ExistingKey_UpdatesPayload()
        {
            using var wrapper = CreateLiteDbWrapper();
            wrapper.Insert(new LiteDbConnectorEntity("k1", "old", null));

            var updated = wrapper.Update(new LiteDbConnectorEntity("k1", "new", null));

            Assert.IsTrue(updated);
            Assert.AreEqual("new", wrapper.Get("k1").Payload);
            Assert.HasCount(1, wrapper.GetAll());
        }

        [TestMethod]
        public void LiteDb_Update_MissingKey_ReturnsFalse()
        {
            using var wrapper = CreateLiteDbWrapper();

            Assert.IsFalse(wrapper.Update(new LiteDbConnectorEntity("missing", "v", null)));
        }

        [TestMethod]
        public void LiteDb_ExpiredEntity_IsNotReturned()
        {
            using var wrapper = CreateLiteDbWrapper();
            var expired = new LiteDbConnectorEntity("exp", "v", null) { ExpiresAt = DateTime.UtcNow.AddMinutes(-1) };
            wrapper.Insert(expired);
            wrapper.Insert(new LiteDbConnectorEntity("ok", "v", TimeSpan.FromMinutes(5)));

            Assert.IsNull(wrapper.Get("exp"));
            Assert.IsFalse(wrapper.Exists("exp"));
            Assert.IsTrue(wrapper.Exists("ok"));
            Assert.HasCount(1, wrapper.GetAll());
            Assert.HasCount(1, wrapper.Query(_ => true));
        }

        #endregion

        private static IConfiguration BuildConfiguration(Dictionary<string, string> values) =>
            new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }
}
