// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;
using System.Linq;

namespace SharpConnector.Configuration
{
    /// <summary>
    /// LiteDbConfig configuration class.
    /// </summary>
    public class LiteDbConfig : IConnectorConfig
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; private set; }
        public int DatabaseNumber { get; private set; }

        public LiteDbConfig(IConfiguration section)
        {
            var sectionChildren = section.GetChildren();
            var configurationSections = sectionChildren.ToList();

            var sectionChildrenConnectionString = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.connectionstring.ToString());
            ConnectionString = sectionChildrenConnectionString?.Value;

            var sectionChildrenDatabaseName = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.databasename.ToString());
            DatabaseName = sectionChildrenDatabaseName?.Value;
        }
    }
}
