// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using LiteDB;
using SharpConnector.Configuration;
using SharpConnector.Entities;

namespace SharpConnector.Connectors.LiteDb
{
    public class LiteDbAccess
    {
        public ILiteCollection<LiteDbConnectorEntity> Collection { get; }

        /// <summary>
        /// Create a new LiteDbAccess instance.
        /// </summary>
        /// <param name="liteDbConfig">The LiteDb configuration.</param>
        public LiteDbAccess(LiteDbConfig liteDbConfig)
        {
            var liteDatabase = new LiteDatabase(liteDbConfig.ConnectionString);
            var collectionName = liteDbConfig.CollectionName;
            Collection = liteDatabase.GetCollection<LiteDbConnectorEntity>(collectionName);
        }
    }
}
