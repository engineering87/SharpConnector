// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;
using System.Linq;

namespace SharpConnector.Configuration
{
    /// <summary>
    /// MongoDb configuration class.
    /// </summary>
    public class MongoDbConfig : IConnectorConfig
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public int DatabaseNumber { get; private set; }

        public MongoDbConfig(IConfiguration section)
        {
            var sectionChildren = section.GetChildren();
            var configurationSections = sectionChildren.ToList();

            var sectionChildrenConnectionString = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.connectionstring.ToString());
            ConnectionString = sectionChildrenConnectionString?.Value;

            var sectionChildrenDatabaseName = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.databasename.ToString());
            DatabaseName = sectionChildrenDatabaseName?.Value;

            var sectionChildrenCollectionName = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.collectionname.ToString());
            CollectionName = sectionChildrenCollectionName?.Value;
        }
    }
}
