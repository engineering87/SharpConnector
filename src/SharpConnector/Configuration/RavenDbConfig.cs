// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Interfaces;
using System.Linq;
using SharpConnector.Enums;

namespace SharpConnector.Configuration
{
    /// <summary>
    /// RavenDbConfig configuration class.
    /// </summary>
    public class RavenDbConfig : IConnectorConfig
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }

        // Not used
        public string CollectionName { get; private set; }
        public int DatabaseNumber { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string BucketName { get; private set; }

        public RavenDbConfig(IConfiguration section)
        {
            var sectionChildren = section.GetChildren();
            var configurationSections = sectionChildren.ToList();

            var sectionChildrenConnectionString = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.connectionstring.ToString());
            ConnectionString = sectionChildrenConnectionString?.Value;

            var sectionChildrenDatabaseName = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.databasename.ToString());
            DatabaseName = sectionChildrenDatabaseName?.Value.Trim();
        }
    }
}
