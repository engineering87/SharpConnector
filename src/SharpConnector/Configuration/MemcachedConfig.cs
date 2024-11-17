using System.Linq;
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;

namespace SharpConnector.Configuration
{
    public class MemcachedConfig : IConnectorConfig
    {
        public string ConnectionString { get; }

        // Not used
        public int DatabaseNumber { get; private set; }
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string BucketName { get; private set; }
        public string AccessKey { get; private set; }
        public string SecretKey { get; private set; }
        public string Region { get; private set; }
        public string ServiceUrl { get; private set; }
        public bool UseHttp { get; private set; }
        public string TableName { get; private set; }

        public MemcachedConfig(IConfiguration section)
        {
            var sectionChildren = section.GetChildren();
            var configurationSections = sectionChildren.ToList();

            var sectionChildrenConnectionString = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.connectionstring.ToString());
            ConnectionString = sectionChildrenConnectionString?.Value;
        }
    }
}
