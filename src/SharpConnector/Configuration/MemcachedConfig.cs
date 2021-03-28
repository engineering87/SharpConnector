using System.Linq;
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;

namespace SharpConnector.Configuration
{
    public class MemcachedConfig : IConnectorConfig
    {
        public int DatabaseNumber { get; private set; }
        public string ConnectionString { get; }
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }

        public MemcachedConfig(IConfiguration section)
        {
            var sectionChildren = section.GetChildren();
            var configurationSections = sectionChildren.ToList();

            var sectionChildrenConnectionString = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.connectionstring.ToString());
            ConnectionString = sectionChildrenConnectionString?.Value;
        }
    }
}
