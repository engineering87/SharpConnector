// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharpConnector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            // Sync setups
            _mockClient.Setup(client => client.Insert(It.IsAny<string>(), It.IsAny<string>()))
                       .Returns(true);

            _mockClient.Setup(client => client.Insert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                       .Returns(true);

            _mockClient.Setup(client => client.Get(It.IsAny<string>()))
                       .Returns((string key) => key == "testKey" ? "payload" : null);

            _mockClient.Setup(client => client.GetAll())
                       .Returns(new List<string> { "payload1", "payload2" });

            _mockClient.Setup(client => client.Exists(It.IsAny<string>()))
                       .Returns((string key) => key == "testKey");

            _mockClient.Setup(client => client.Query(It.IsAny<Func<string, bool>>()))
                       .Returns((Func<string, bool> filter) =>
                           new List<string> { "payload1", "payload2", "other" }.Where(filter));

            // Async setups
            _mockClient.Setup(client => client.InsertAsync(
                                    It.IsAny<string>(),
                                    It.IsAny<string>(),
                                    It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

            _mockClient.Setup(client => client.InsertAsync(
                                    It.IsAny<string>(),
                                    It.IsAny<string>(),
                                    It.IsAny<TimeSpan>(),
                                    It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

            _mockClient.Setup(client => client.GetAsync(
                                    It.IsAny<string>(),
                                    It.IsAny<CancellationToken>()))
                       .ReturnsAsync((string key, CancellationToken _) =>
                                        key == "testKey" ? "payload" : null);

            _mockClient.Setup(client => client.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new List<string> { "payload1", "payload2" });

            _mockClient.Setup(client => client.ExistsAsync(
                                    It.IsAny<string>(),
                                    It.IsAny<CancellationToken>()))
                       .ReturnsAsync((string key, CancellationToken _) => key == "testKey");

            _mockClient.Setup(client => client.QueryAsync(
                                    It.IsAny<Func<string, bool>>(),
                                    It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Func<string, bool> filter, CancellationToken _) =>
                           new List<string> { "payload1", "payload2", "other" }.Where(filter));

            _mockClient.Setup(client => client.InsertManyAsync(It.IsAny<Dictionary<string, string>>()))
                       .ReturnsAsync(true);

            _mockClient.Setup(client => client.InsertManyAsync(It.IsAny<Dictionary<string, string>>(), It.IsAny<TimeSpan>()))
                       .ReturnsAsync(true);

            _mockClient.Setup(client => client.InsertManyAsync(
                                    It.IsAny<IEnumerable<string>>(),
                                    It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

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
        public void Insert_WithExpiration()
        {
            const string key = "testKey";
            var result = _sharpConnectorClient.Insert(key, "payload", TimeSpan.FromMinutes(5));
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
        public async Task InsertAsync_WithExpiration()
        {
            const string key = "testKey";
            var result = await _sharpConnectorClient.InsertAsync(key, "payload", TimeSpan.FromMinutes(5));
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
        public void InsertMany_WithExpiration()
        {
            var dictionary = new Dictionary<string, string>()
            {
                { "key", "payload" },
                { "key2", "payload2" }
            };

            _mockClient.Setup(client => client.InsertMany(dictionary, It.IsAny<TimeSpan>())).Returns(true);

            var result = _sharpConnectorClient.InsertMany(dictionary, TimeSpan.FromMinutes(10));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task InsertManyAsync_Dictionary()
        {
            var dictionary = new Dictionary<string, string>()
            {
                { "key", "payload" },
                { "key2", "payload2" }
            };

            var result = await _sharpConnectorClient.InsertManyAsync(dictionary);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task InsertManyAsync_Dictionary_WithExpiration()
        {
            var dictionary = new Dictionary<string, string>()
            {
                { "key", "payload" },
                { "key2", "payload2" }
            };

            var result = await _sharpConnectorClient.InsertManyAsync(dictionary, TimeSpan.FromMinutes(10));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task InsertManyAsync_Enumerable()
        {
            var values = new List<string> { "payload1", "payload2", "payload3" };
            var result = await _sharpConnectorClient.InsertManyAsync(values);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Get()
        {
            const string key = "testKey";
            var insert = _sharpConnectorClient.Insert(key, "payload");
            Assert.IsTrue(insert);
            var obj = _sharpConnectorClient.Get(key);
            Assert.AreEqual("payload", obj);
        }

        [TestMethod]
        public void Get_ReturnsNull_WhenKeyNotFound()
        {
            var obj = _sharpConnectorClient.Get("nonExistentKey");
            Assert.IsNull(obj);
        }

        [TestMethod]
        public async Task GetAsync()
        {
            const string key = "testKey";
            var insert = _sharpConnectorClient.Insert(key, "payload");
            Assert.IsTrue(insert);
            var obj = await _sharpConnectorClient.GetAsync(key);
            Assert.AreEqual("payload", obj);
        }

        [TestMethod]
        public async Task GetAsync_ReturnsNull_WhenKeyNotFound()
        {
            var obj = await _sharpConnectorClient.GetAsync("nonExistentKey");
            Assert.IsNull(obj);
        }

        [TestMethod]
        public void GetAll()
        {
            var results = _sharpConnectorClient.GetAll();
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public async Task GetAllAsync()
        {
            var results = await _sharpConnectorClient.GetAllAsync();
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void Update()
        {
            const string key = "testKey";
            var insert = _sharpConnectorClient.Insert(key, "payload");
            Assert.IsTrue(insert);
            var obj = _sharpConnectorClient.Get(key);
            Assert.AreEqual("payload", obj);

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
            Assert.AreEqual("payload", obj);

            _mockClient.Setup(client => client.UpdateAsync(
                                        key,
                                        "modPayload",
                                        It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

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
            Assert.AreEqual("payload", obj);

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
            Assert.AreEqual("payload", obj);

            _mockClient.Setup(client => client.DeleteAsync(
                                        key,
                                        It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

            var delete = await _sharpConnectorClient.DeleteAsync(key);
            Assert.IsTrue(delete);
        }

        [TestMethod]
        public void Exists_ReturnsTrue_WhenKeyExists()
        {
            var result = _sharpConnectorClient.Exists("testKey");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Exists_ReturnsFalse_WhenKeyNotFound()
        {
            var result = _sharpConnectorClient.Exists("nonExistentKey");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ExistsAsync_ReturnsTrue_WhenKeyExists()
        {
            var result = await _sharpConnectorClient.ExistsAsync("testKey");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ExistsAsync_ReturnsFalse_WhenKeyNotFound()
        {
            var result = await _sharpConnectorClient.ExistsAsync("nonExistentKey");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Query_ReturnsFilteredResults()
        {
            var results = _sharpConnectorClient.Query(s => s.StartsWith("payload"));
            Assert.IsNotNull(results);
            var list = results.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.All(s => s.StartsWith("payload")));
        }

        [TestMethod]
        public void Query_ReturnsEmpty_WhenNoMatch()
        {
            var results = _sharpConnectorClient.Query(s => s == "nonExistent");
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public async Task QueryAsync_ReturnsFilteredResults()
        {
            var results = await _sharpConnectorClient.QueryAsync(s => s.StartsWith("payload"));
            Assert.IsNotNull(results);
            var list = results.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.All(s => s.StartsWith("payload")));
        }

        [TestMethod]
        public async Task QueryAsync_ReturnsEmpty_WhenNoMatch()
        {
            var results = await _sharpConnectorClient.QueryAsync(s => s == "nonExistent");
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count());
        }
    }
}