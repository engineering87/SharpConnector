// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpConnector.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpConnector.Test
{
    [TestClass]
    public class RedisTests
    {
        private readonly ISharpConnectorClient<string> _sharpConnectorClient;

        public RedisTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.redis.json", optional: true, reloadOnChange: true);

            _sharpConnectorClient = new SharpConnectorClient<string>(builder);
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
            var result = _sharpConnectorClient.InsertMany(dictionary);
            Assert.IsTrue(result);
            var list = _sharpConnectorClient.GetAll();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.ToList().Count >= dictionary.Count);
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
        public void GetAll()
        {
            const string key = "testKey";
            const string otherkey = "testKey2";
            var insert = _sharpConnectorClient.Insert(key, "payload");
            Assert.IsTrue(insert);
            insert = _sharpConnectorClient.Insert(otherkey, "payload");
            Assert.IsTrue(insert);
            var list = _sharpConnectorClient.GetAll();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.ToList().Count >= 2);
        }

        [TestMethod]
        public void Update()
        {
            const string key = "testKey";
            var insert = _sharpConnectorClient.Insert(key, "payload");
            Assert.IsTrue(insert);
            var obj = _sharpConnectorClient.Get(key);
            Assert.AreEqual(obj, "payload");
            var update = _sharpConnectorClient.Update(key, "modPayload");
            Assert.IsTrue(update);
            obj = _sharpConnectorClient.Get(key);
            Assert.AreEqual(obj, "modPayload");
        }

        [TestMethod]
        public async Task UpdateAsync()
        {
            const string key = "testKey";
            var insert = await _sharpConnectorClient.InsertAsync(key, "payload");
            Assert.IsTrue(insert);
            var obj = await _sharpConnectorClient.GetAsync(key);
            Assert.AreEqual(obj, "payload");
            var update = await _sharpConnectorClient.UpdateAsync(key, "modPayload");
            Assert.IsTrue(update);
            obj = await _sharpConnectorClient.GetAsync(key);
            Assert.AreEqual(obj, "modPayload");
        }

        [TestMethod]
        public void Delete()
        {
            const string key = "testKey";
            var insert = _sharpConnectorClient.Insert(key, "payload");
            Assert.IsTrue(insert);
            var obj = _sharpConnectorClient.Get(key);
            Assert.AreEqual(obj, "payload");
            var delete = _sharpConnectorClient.Delete(key);
            Assert.IsTrue(delete);
            obj = _sharpConnectorClient.Get(key);
            Assert.IsNull(obj);
        }

        [TestMethod]
        public async Task DeleteAsync()
        {
            const string key = "testKey";
            var insert = await _sharpConnectorClient.InsertAsync(key, "payload");
            Assert.IsTrue(insert);
            var obj = await _sharpConnectorClient.GetAsync(key);
            Assert.AreEqual(obj, "payload");
            var delete = await _sharpConnectorClient.DeleteAsync(key);
            Assert.IsTrue(delete);
            obj = await _sharpConnectorClient.GetAsync(key);
            Assert.IsNull(obj);
        }

    }
}
