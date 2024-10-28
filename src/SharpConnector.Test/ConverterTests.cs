// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpConnector.Entities;
using SharpConnector.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SharpConnector.Tests
{
    [TestClass]
    public class ConverterTests
    {
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
    }
}
