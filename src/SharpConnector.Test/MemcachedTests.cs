﻿using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpConnector.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SharpConnector.Test
{
    [TestClass]
    public class MemcachedTests
    {
        private readonly ISharpConnectorClient<string> _sharpConnectorClient;

        public MemcachedTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.memcached.json", optional: true, reloadOnChange: true);

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
    }
}
