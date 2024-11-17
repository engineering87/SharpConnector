// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;
using System.Linq;

namespace SharpConnector.Configuration
{
    public class DynamoDbConfig : IConnectorConfig
    {
        public string AccessKey { get; }
        public string SecretKey { get; }
        public string Region { get; }
        public string ServiceUrl { get; }
        public bool UseHttp { get; }
        public string TableName { get; }

        // Not used
        public string ConnectionString { get; private set; }
        public int DatabaseNumber { get; private set; }
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string BucketName { get; private set; }

        public DynamoDbConfig(IConfiguration section)
        {
            var sectionChildren = section.GetChildren();
            var configurationSections = sectionChildren.ToList();

            var sectionChildrenAccessKey = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.accesskey.ToString());
            AccessKey = sectionChildrenAccessKey?.Value;

            var sectionChildrenSecretKey = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.secretkey.ToString());
            SecretKey = sectionChildrenSecretKey?.Value;

            var sectionChildrenRegion = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.region.ToString());
            Region = sectionChildrenRegion?.Value;

            var sectionChildrenServiceUrl = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.serviceurl.ToString());
            ServiceUrl = sectionChildrenServiceUrl?.Value;

            var sectionChildrenUseHttp = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.usehttp.ToString());
            bool.TryParse(sectionChildrenUseHttp?.Value, out var useHttp);
            UseHttp = useHttp;

            var sectionChildrenTableName = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.tablename.ToString());
            TableName = sectionChildrenTableName?.Value;
        }
    }
}
