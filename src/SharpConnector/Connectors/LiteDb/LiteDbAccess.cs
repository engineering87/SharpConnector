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
        private readonly LiteDatabase _liteDatabase;

        private bool _disposed;

        /// <summary>
        /// Create a new LiteDbAccess instance.
        /// </summary>
        /// <param name="liteDbConfig">The LiteDb configuration.</param>
        public LiteDbAccess(LiteDbConfig liteDbConfig)
        {
            if (liteDbConfig == null)
            {
                throw new ArgumentNullException(nameof(liteDbConfig));
            }

            _liteDatabase = new LiteDatabase(liteDbConfig.ConnectionString);
            var collectionName = liteDbConfig.CollectionName;
            Collection = _liteDatabase.GetCollection<LiteDbConnectorEntity>(collectionName);
        }

        /// <summary>
        /// Dispose the LiteDatabase instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _liteDatabase?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
