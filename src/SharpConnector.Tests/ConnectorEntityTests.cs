// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpConnector.Entities;
using System;

namespace SharpConnector.Tests
{
    [TestClass]
    public class ConnectorEntityTests
    {
        #region Constructor tests

        [TestMethod]
        public void Constructor_KeyAndPayload_SetsProperties()
        {
            var entity = new ConnectorEntity("key1", "payload1");

            Assert.AreEqual("key1", entity.Key);
            Assert.AreEqual("payload1", entity.Payload);
            Assert.IsNull(entity.Expiration);
        }

        [TestMethod]
        public void Constructor_KeyPayloadExpiration_SetsProperties()
        {
            var expiration = TimeSpan.FromMinutes(10);
            var entity = new ConnectorEntity("key1", "payload1", expiration);

            Assert.AreEqual("key1", entity.Key);
            Assert.AreEqual("payload1", entity.Payload);
            Assert.AreEqual(expiration, entity.Expiration);
        }

        [TestMethod]
        public void Constructor_NullExpiration_SetsExpirationToNull()
        {
            var entity = new ConnectorEntity("key1", "payload1", null);

            Assert.AreEqual("key1", entity.Key);
            Assert.AreEqual("payload1", entity.Payload);
            Assert.IsNull(entity.Expiration);
        }

        [TestMethod]
        public void Constructor_NullKey_ThrowsArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new ConnectorEntity(null, "payload1"));
        }

        [TestMethod]
        public void Constructor_EmptyKey_ThrowsArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new ConnectorEntity("", "payload1"));
        }

        [TestMethod]
        public void Constructor_NullPayload_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new ConnectorEntity("key1", null));
        }

        [TestMethod]
        public void Constructor_WithExpiration_NullKey_ThrowsArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
                new ConnectorEntity(null, "payload", TimeSpan.FromMinutes(1)));
        }

        [TestMethod]
        public void Constructor_WithExpiration_NullPayload_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                new ConnectorEntity("key1", null, TimeSpan.FromMinutes(1)));
        }

        [TestMethod]
        public void DefaultConstructor_PropertiesAreNull()
        {
            var entity = new ConnectorEntity();

            Assert.IsNull(entity.Key);
            Assert.IsNull(entity.Payload);
            Assert.IsNull(entity.Expiration);
        }

        #endregion

        #region Equals tests

        [TestMethod]
        public void Equals_SameKeyAndPayload_ReturnsTrue()
        {
            var entity1 = new ConnectorEntity("key1", "payload1");
            var entity2 = new ConnectorEntity("key1", "payload1");

            Assert.IsTrue(entity1.Equals(entity2));
        }

        [TestMethod]
        public void Equals_DifferentKey_ReturnsFalse()
        {
            var entity1 = new ConnectorEntity("key1", "payload1");
            var entity2 = new ConnectorEntity("key2", "payload1");

            Assert.IsFalse(entity1.Equals(entity2));
        }

        [TestMethod]
        public void Equals_DifferentPayload_ReturnsFalse()
        {
            var entity1 = new ConnectorEntity("key1", "payload1");
            var entity2 = new ConnectorEntity("key1", "payload2");

            Assert.IsFalse(entity1.Equals(entity2));
        }

        [TestMethod]
        public void Equals_DifferentExpiration_ReturnsFalse()
        {
            var entity1 = new ConnectorEntity("key1", "payload1", TimeSpan.FromMinutes(5));
            var entity2 = new ConnectorEntity("key1", "payload1", TimeSpan.FromMinutes(10));

            Assert.IsFalse(entity1.Equals(entity2));
        }

        [TestMethod]
        public void Equals_SameExpiration_ReturnsTrue()
        {
            var expiration = TimeSpan.FromMinutes(5);
            var entity1 = new ConnectorEntity("key1", "payload1", expiration);
            var entity2 = new ConnectorEntity("key1", "payload1", expiration);

            Assert.IsTrue(entity1.Equals(entity2));
        }

        [TestMethod]
        public void Equals_Null_ReturnsFalse()
        {
            var entity = new ConnectorEntity("key1", "payload1");
            Assert.IsFalse(entity.Equals(null));
        }

        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var entity = new ConnectorEntity("key1", "payload1");
            Assert.IsFalse(entity.Equals("not an entity"));
        }

        #endregion

        #region GetHashCode tests

        [TestMethod]
        public void GetHashCode_EqualEntities_ReturnsSameHash()
        {
            var entity1 = new ConnectorEntity("key1", "payload1", TimeSpan.FromMinutes(5));
            var entity2 = new ConnectorEntity("key1", "payload1", TimeSpan.FromMinutes(5));

            Assert.AreEqual(entity1.GetHashCode(), entity2.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_DifferentEntities_ReturnsDifferentHash()
        {
            var entity1 = new ConnectorEntity("key1", "payload1");
            var entity2 = new ConnectorEntity("key2", "payload2");

            Assert.AreNotEqual(entity1.GetHashCode(), entity2.GetHashCode());
        }

        #endregion

        #region ToString tests

        [TestMethod]
        public void ToString_ReturnsJsonString()
        {
            var entity = new ConnectorEntity("key1", "payload1");
            var json = entity.ToString();

            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("key1"));
            Assert.IsTrue(json.Contains("payload1"));
        }

        #endregion

        #region LiteDbConnectorEntity tests

        [TestMethod]
        public void LiteDbConnectorEntity_Constructor_SetsProperties()
        {
            var expiration = TimeSpan.FromHours(1);
            var entity = new LiteDbConnectorEntity("key1", "payload1", expiration);

            Assert.AreEqual("key1", entity.Key);
            Assert.AreEqual("payload1", entity.Payload);
            Assert.AreEqual(expiration, entity.Expiration);
        }

        [TestMethod]
        public void LiteDbConnectorEntity_DefaultConstructor_PropertiesAreNull()
        {
            var entity = new LiteDbConnectorEntity();

            Assert.IsNull(entity.Key);
            Assert.IsNull(entity.Payload);
            Assert.IsNull(entity.Expiration);
        }

        #endregion

        #region MongoConnectorEntity tests

        [TestMethod]
        public void MongoConnectorEntity_Constructor_SetsProperties()
        {
            var expiration = TimeSpan.FromMinutes(30);
            var entity = new MongoConnectorEntity("key1", "payload1", expiration);

            Assert.AreEqual("key1", entity.Key);
            Assert.AreEqual("payload1", entity.Payload);
            Assert.AreEqual(expiration, entity.Expiration);
        }

        [TestMethod]
        public void MongoConnectorEntity_DefaultConstructor_PropertiesAreNull()
        {
            var entity = new MongoConnectorEntity();

            Assert.IsNull(entity.Key);
            Assert.IsNull(entity.Payload);
            Assert.IsNull(entity.Expiration);
        }

        #endregion
    }
}
