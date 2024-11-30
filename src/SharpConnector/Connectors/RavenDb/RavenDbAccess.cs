// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Raven.Client.Documents;
using SharpConnector.Configuration;
using System;

namespace SharpConnector.Connectors.RavenDb
{
    public class RavenDbAccess : IDisposable
    {
        private static Lazy<IDocumentStore> DocumentStore { get; set; }

        /// <summary>
        /// Create a new RavenDbAccess instance.
        /// </summary>
        /// <param name="ravenDbConfig"></param>
        public RavenDbAccess(RavenDbConfig ravenDbConfig)
        {
            if (DocumentStore == null)
            {
                DocumentStore = new Lazy<IDocumentStore>(() =>
                {
                    var documentStore = new DocumentStore
                    {
                        Urls = [ravenDbConfig.ConnectionString],
                        Database = ravenDbConfig.DatabaseName
                    };

                    documentStore.Initialize();
                    return documentStore;
                });
            }
        }

        /// <summary>
        /// Return the DocumentStore.
        /// </summary>
        public IDocumentStore Store => DocumentStore.Value;

        /// <summary>
        /// Dispose of the DocumentStore.
        /// </summary>
        public void Dispose()
        {
            if (DocumentStore.IsValueCreated)
            {
                DocumentStore.Value.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
