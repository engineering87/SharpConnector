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
        public IMongoCollection<MongoConnectorEntity> Collection { get; }
        private readonly MongoClient _client;

        /// <summary>
        /// Create a new MongoDbAccess instance.
        /// </summary>
        /// <param name="mongoDbConfig">The MongoDb configuration.</param>
        public MongoDbAccess(MongoDbConfig mongoDbConfig)
        {
            if (mongoDbConfig == null)
            {
                throw new ArgumentNullException(nameof(mongoDbConfig), "MongoDB configuration cannot be null.");
            }

            _client = new MongoClient(mongoDbConfig.ConnectionString);
            var database = _client.GetDatabase(mongoDbConfig.DatabaseName);
            Collection = database.GetCollection<MongoConnectorEntity>(mongoDbConfig.CollectionName);
        }

        /// <summary>
        /// Dispose of the MongoClient.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
