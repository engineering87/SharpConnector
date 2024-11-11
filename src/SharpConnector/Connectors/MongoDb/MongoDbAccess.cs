// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using MongoDB.Driver;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;

namespace SharpConnector.Connectors.MongoDb
{
    public class MongoDbAccess : IDisposable
    {
        public IMongoCollection<ConnectorEntity> Collection { get; }
        private MongoClient _client;

        /// <summary>
        /// Create a new MongoDbAccess instance.
        /// </summary>
        /// <param name="mongoDbConfig">The MongoDb configuration.</param>
        public MongoDbAccess(MongoDbConfig mongoDbConfig)
        {
            var databaseName = mongoDbConfig.DatabaseName;
            var collectionName = mongoDbConfig.CollectionName;
            _client = new MongoClient(mongoDbConfig.ConnectionString);
            var db = _client.GetDatabase(databaseName);
            Collection = db.GetCollection<ConnectorEntity>(collectionName);
        }

        /// <summary>
        /// Dispose of the MongoClient.
        /// </summary>
        public void Dispose()
        {
            _client = null;
            GC.SuppressFinalize(this);
        }
    }
}
