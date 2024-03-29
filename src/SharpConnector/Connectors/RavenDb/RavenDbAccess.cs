﻿// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Raven.Client.Documents;
using SharpConnector.Configuration;
using System;

namespace SharpConnector.Connectors.RavenDb
{
    public class RavenDbAccess
    {
        private static Lazy<IDocumentStore> DocumentStore { get; set; }

        /// <summary>
        /// Create a new RavenDbAccess instance.
        /// </summary>
        /// <param name="ravenDbConfig"></param>
        public RavenDbAccess(RavenDbConfig ravenDbConfig)
        {
            IDocumentStore documentStore = new DocumentStore
            {
                Urls = new[] { ravenDbConfig.ConnectionString },
                Database = ravenDbConfig.DatabaseName
            };

            documentStore.Initialize();
            DocumentStore = new Lazy<IDocumentStore>(documentStore);
        }

        /// <summary>
        /// Return the DocumentStore.
        /// </summary>
        public IDocumentStore Store
        {
            get { return DocumentStore.Value; }
        }
    }
}
