// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using MongoDB.Driver;
using SharpConnector.Configuration;
using SharpConnector.Entities;

namespace SharpConnector.Connectors.MongoDb
{
    public class MongoDbAccess
    {
        public IMongoCollection<ConnectorEntity> Collection { get; }

        /// <summary>
        /// Create a new MongoDbAccess instance.
        /// </summary>
        /// <param name="mongoDbConfig">The MongoDb configuration.</param>
        public MongoDbAccess(MongoDbConfig mongoDbConfig)
        {
            var databaseName = mongoDbConfig.DatabaseName;
            var collectionName = mongoDbConfig.CollectionName;
            var client = new MongoClient(mongoDbConfig.ConnectionString);
            var db = client.GetDatabase(databaseName);
            Collection = db.GetCollection<ConnectorEntity>(collectionName);
        }
    }
}
