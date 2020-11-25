// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpConnector.Operations;

namespace SharpConnector.Test
{
    [TestClass]
    public class MongoDbTests
    {
        private readonly SharpConnectorClient<string> _sharpConnectorClient;

        public MongoDbTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.mongodb.json", optional: true, reloadOnChange: true);

            _sharpConnectorClient = new SharpConnectorClient<string>(ConnectorTypes.MongoDb, builder);
        }

        [TestMethod]
        public void Insert()
        {
            const string key = "testKey";
            var result = _sharpConnectorClient.Insert(key, "payload");
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
