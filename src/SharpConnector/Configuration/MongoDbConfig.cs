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
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }
        public int DatabaseNumber { get; private set; }
        public string ConnectionString { get; private set; }

        public MongoDbConfig(IConfigurationSection section)
        {
            var sectionChildren = section.GetChildren();

            var sectionChildrenDatabasename = sectionChildren.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.databasename.ToString());
            DatabaseName = sectionChildrenDatabasename?.Value;

            var sectionChildrenCollectionname = sectionChildren.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.collectionname.ToString());
            CollectionName = sectionChildrenCollectionname?.Value;

            var sectionChildrenConnectionString = sectionChildren.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.connectionstring.ToString());
            ConnectionString = sectionChildrenConnectionString?.Value;
        }
    }
}
