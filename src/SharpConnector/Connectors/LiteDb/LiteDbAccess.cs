// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using LiteDB;
using SharpConnector.Configuration;
using SharpConnector.Entities;
using System;

namespace SharpConnector.Connectors.LiteDb
{
    public class LiteDbAccess : IDisposable
    {
        public ILiteCollection<LiteDbConnectorEntity> Collection { get; }
        private LiteDatabase _liteDatabase;

        /// <summary>
        /// Create a new LiteDbAccess instance.
        /// </summary>
        /// <param name="liteDbConfig">The LiteDb configuration.</param>
        public LiteDbAccess(LiteDbConfig liteDbConfig)
        {
            _liteDatabase = new LiteDatabase(liteDbConfig.ConnectionString);
            var collectionName = liteDbConfig.CollectionName;
            Collection = _liteDatabase.GetCollection<LiteDbConnectorEntity>(collectionName);
        }

        /// <summary>
        /// Dispose the LiteDatabase instance.
        /// </summary>
        public void Dispose()
        {
            _liteDatabase?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
