// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;
using System.Linq;

namespace SharpConnector.Configuration
{
    /// <summary>
    /// Redis configuration class.
    /// </summary>
    public class RedisConfig : IConnectorConfig
    {
        public string ConnectionString { get; }
        public int DatabaseNumber { get; } // 0 default
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }

        public RedisConfig(IConfiguration section)
        {
            var sectionChildren = section.GetChildren();
            var configurationSections = sectionChildren.ToList();

            var sectionChildrenConnectionString = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.connectionstring.ToString());
            ConnectionString = sectionChildrenConnectionString?.Value;

            var sectionChildrenDbNumber = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.databasenumber.ToString());
            int.TryParse(sectionChildrenDbNumber?.Value, out var dbNumber);
            DatabaseNumber = dbNumber;
        }
    }
}
