// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharpConnector.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpConnector.Tests
{
    [TestClass]
    public class ConnectorTests
    {
        private readonly ISharpConnectorClient<string> _sharpConnectorClient;
        private readonly Mock<ISharpConnectorClient<string>> _mockClient;

        public ConnectorTests()
        {
            _mockClient = new Mock<ISharpConnectorClient<string>>();

            _mockClient.Setup(client => client.Insert(It.IsAny<string>(), It.IsAny<string>()))
                       .Returns(true);

            _mockClient.Setup(client => client.InsertAsync(It.IsAny<string>(), It.IsAny<string>()))
                       .ReturnsAsync(true);

            _mockClient.Setup(client => client.Get(It.IsAny<string>()))
                       .Returns((string key) => key == "testKey" ? "payload" : null);

            _mockClient.Setup(client => client.GetAsync(It.IsAny<string>()))
                       .ReturnsAsync((string key) => key == "testKey" ? "payload" : null);

            _sharpConnectorClient = _mockClient.Object;
        }

        [TestMethod]
        public void Insert()
        {
            const string key = "testKey";
            var result = _sharpConnectorClient.Insert(key, "payload");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task InsertAsync()
        {
            const string key = "testKey";
            var result = await _sharpConnectorClient.InsertAsync(key, "payload");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InsertMany()
        {
            var dictionary = new Dictionary<string, string>()
            {
                { "key", "payload" },
                { "key2", "payload2" },
                { "key3", "payload3" }
            };

            _mockClient.Setup(client => client.InsertMany(dictionary)).Returns(true);

            var result = _sharpConnectorClient.InsertMany(dictionary);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Get()
        {
            const string key = "testKey";
            var insert = _sharpConnectorClient.Insert(key, "payload");
            Assert.IsTrue(insert);
            var obj = _sharpConnectorClient.Get(key);
            Assert.AreEqual(obj, "payload");
        }

        [TestMethod]
        public async Task GetAsync()
        {
            const string key = "testKey";
            var insert = _sharpConnectorClient.Insert(key, "payload");
            Assert.IsTrue(insert);
            var obj = await _sharpConnectorClient.GetAsync(key);
            Assert.AreEqual(obj, "payload");
        }

        [TestMethod]
        public void Update()
        {
            const string key = "testKey";
            var insert = _sharpConnectorClient.Insert(key, "payload");
            Assert.IsTrue(insert);
            var obj = _sharpConnectorClient.Get(key);
            Assert.AreEqual(obj, "payload");

            // Set up mock for update method
            _mockClient.Setup(client => client.Update(key, "modPayload")).Returns(true);
            var update = _sharpConnectorClient.Update(key, "modPayload");
            Assert.IsTrue(update);
        }

        [TestMethod]
        public async Task UpdateAsync()
        {
            const string key = "testKey";
            var insert = await _sharpConnectorClient.InsertAsync(key, "payload");
            Assert.IsTrue(insert);
            var obj = await _sharpConnectorClient.GetAsync(key);
            Assert.AreEqual(obj, "payload");

            // Mock async update behavior
            _mockClient.Setup(client => client.UpdateAsync(key, "modPayload")).ReturnsAsync(true);
            var update = await _sharpConnectorClient.UpdateAsync(key, "modPayload");
            Assert.IsTrue(update);
        }

        [TestMethod]
        public void Delete()
        {
            const string key = "testKey";
            var insert = _sharpConnectorClient.Insert(key, "payload");
            Assert.IsTrue(insert);
            var obj = _sharpConnectorClient.Get(key);
            Assert.AreEqual(obj, "payload");

            // Mock delete behavior
            _mockClient.Setup(client => client.Delete(key)).Returns(true);
            var delete = _sharpConnectorClient.Delete(key);
            Assert.IsTrue(delete);
        }

        [TestMethod]
        public async Task DeleteAsync()
        {
            const string key = "testKey";
            var insert = await _sharpConnectorClient.InsertAsync(key, "payload");
            Assert.IsTrue(insert);
            var obj = await _sharpConnectorClient.GetAsync(key);
            Assert.AreEqual(obj, "payload");

            // Mock async delete behavior
            _mockClient.Setup(client => client.DeleteAsync(key)).ReturnsAsync(true);
            var delete = await _sharpConnectorClient.DeleteAsync(key);
            Assert.IsTrue(delete);
        }
    }
}
