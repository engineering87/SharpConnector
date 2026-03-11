// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpConnector.Tests
{
    [TestClass]
    public class ConverterTests
    {
        #region ToPayloadObject

        [TestMethod]
        public void ToPayloadObject_ConnectorEntity_ReturnsCorrectObject()
        {
            // Arrange
            var connectorEntity = new ConnectorEntity("testKey", "testPayload");

            // Act
            var result = connectorEntity.ToPayloadObject<string>();

            // Assert
            Assert.AreEqual("testPayload", result);
        }

        [TestMethod]
        public void ToPayloadObject_ConnectorEntity_ReturnsCorrectInt()
        {
            var connectorEntity = new ConnectorEntity("testKey", 42);
            var result = connectorEntity.ToPayloadObject<int>();
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void ToPayloadObject_LiteDbConnectorEntity_ReturnsCorrectObject()
        {
            var entity = new LiteDbConnectorEntity("testKey", "testPayload", null);
            var result = entity.ToPayloadObject<string>();
            Assert.AreEqual("testPayload", result);
        }

        [TestMethod]
        public void ToPayloadObject_JObjectPayload_ReturnsDeserializedObject()
        {
            // Simulates what happens after JSON round-trip deserialization
            var jObj = JObject.FromObject(new { Name = "Test", Value = 42 });
            var entity = new ConnectorEntity { Key = "k1", Payload = jObj };

            var result = entity.ToPayloadObject<TestPayload>();

            Assert.IsNotNull(result);
            Assert.AreEqual("Test", result.Name);
            Assert.AreEqual(42, result.Value);
        }

        [TestMethod]
        public void ToPayloadObject_JValuePayload_ReturnsConvertedPrimitive()
        {
            var jVal = new JValue("hello");
            var entity = new ConnectorEntity { Key = "k1", Payload = jVal };

            var result = entity.ToPayloadObject<string>();
            Assert.AreEqual("hello", result);
        }

        #endregion

        #region ToPayloadList

        [TestMethod]
        public void ToPayloadList_ConnectorEntities_ReturnsCorrectList()
        {
            var entities = new List<ConnectorEntity>
            {
                new ConnectorEntity("k1", "v1"),
                new ConnectorEntity("k2", "v2"),
                new ConnectorEntity("k3", "v3")
            };

            var result = entities.ToPayloadList<string>().ToList();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("v1", result[0]);
            Assert.AreEqual("v2", result[1]);
            Assert.AreEqual("v3", result[2]);
        }

        [TestMethod]
        public void ToPayloadList_SkipsNullPayloads()
        {
            var entities = new List<ConnectorEntity>
            {
                new ConnectorEntity("k1", "v1"),
                new ConnectorEntity { Key = "k2", Payload = null },
                new ConnectorEntity("k3", "v3")
            };

            var result = entities.ToPayloadList<string>().ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("v1", result[0]);
            Assert.AreEqual("v3", result[1]);
        }

        [TestMethod]
        public void ToPayloadList_EmptyList_ReturnsEmpty()
        {
            var entities = new List<ConnectorEntity>();
            var result = entities.ToPayloadList<string>().ToList();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ToPayloadList_LiteDbConnectorEntities_ReturnsCorrectList()
        {
            var entities = new List<LiteDbConnectorEntity>
            {
                new LiteDbConnectorEntity("k1", "v1", null),
                new LiteDbConnectorEntity("k2", "v2", null)
            };

            var result = entities.ToPayloadList<string>().ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("v1", result[0]);
            Assert.AreEqual("v2", result[1]);
        }

        #endregion

        #region ToConnectorEntityList

        [TestMethod]
        public void ToConnectorEntityList_ValidDictionary_ReturnsCorrectList()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" },
                { "", "value3" }, // This should be ignored
                { "key3", null }  // This should be ignored
            };

            // Act
            var result = dictionary.ToConnectorEntityList<string>();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("key1", result[0].Key);
            Assert.AreEqual("value1", result[0].Payload);
            Assert.AreEqual("key2", result[1].Key);
            Assert.AreEqual("value2", result[1].Payload);
        }

        [TestMethod]
        public void ToConnectorEntityList_NullDictionary_ReturnsEmptyList()
        {
            Dictionary<string, string> dictionary = null;
            var result = dictionary.ToConnectorEntityList();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ToConnectorEntityList_WithExpiration_SetsExpirationOnAll()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            var expiration = TimeSpan.FromMinutes(30);

            var result = dictionary.ToConnectorEntityList(expiration);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(expiration, result[0].Expiration);
            Assert.AreEqual(expiration, result[1].Expiration);
        }

        [TestMethod]
        public void ToConnectorEntityList_WithoutExpiration_ExpirationIsNull()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "key1", "value1" }
            };

            var result = dictionary.ToConnectorEntityList();

            Assert.AreEqual(1, result.Count);
            Assert.IsNull(result[0].Expiration);
        }

        #endregion

        #region ToLiteDbConnectorEntityList

        [TestMethod]
        public void ToLiteDbConnectorEntityList_ValidDictionary_ReturnsCorrectList()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" },
                { "", "value3" }, // This should be ignored
                { "key3", null }  // This should be ignored
            };

            // Act
            var result = dictionary.ToLiteDbConnectorEntityList<string>();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("key1", result[0].Key);
            Assert.AreEqual("value1", result[0].Payload);
            Assert.AreEqual("key2", result[1].Key);
            Assert.AreEqual("value2", result[1].Payload);
        }

        [TestMethod]
        public void ToLiteDbConnectorEntityList_NullDictionary_ReturnsEmptyList()
        {
            Dictionary<string, string> dictionary = null;
            var result = dictionary.ToLiteDbConnectorEntityList();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ToLiteDbConnectorEntityList_WithExpiration_SetsExpirationOnAll()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "key1", "value1" }
            };
            var expiration = TimeSpan.FromHours(1);

            var result = dictionary.ToLiteDbConnectorEntityList(expiration);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expiration, result[0].Expiration);
        }

        #endregion

        #region ToMongoDbConnectorEntityList

        [TestMethod]
        public void ToMongoDbConnectorEntityList_ValidDictionary_ReturnsCorrectList()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" },
                { "", "value3" },
                { "key3", null }
            };

            var result = dictionary.ToMongoDbConnectorEntityList<string>();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("key1", result[0].Key);
            Assert.AreEqual("value1", result[0].Payload);
            Assert.AreEqual("key2", result[1].Key);
            Assert.AreEqual("value2", result[1].Payload);
        }

        [TestMethod]
        public void ToMongoDbConnectorEntityList_NullDictionary_ReturnsEmptyList()
        {
            Dictionary<string, string> dictionary = null;
            var result = dictionary.ToMongoDbConnectorEntityList();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ToMongoDbConnectorEntityList_WithExpiration_SetsExpirationOnAll()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            var expiration = TimeSpan.FromMinutes(15);

            var result = dictionary.ToMongoDbConnectorEntityList(expiration);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(expiration, result[0].Expiration);
            Assert.AreEqual(expiration, result[1].Expiration);
        }

        #endregion

        private class TestPayload
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
