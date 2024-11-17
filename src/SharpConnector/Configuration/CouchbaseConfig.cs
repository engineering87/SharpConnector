// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Enums;
using SharpConnector.Interfaces;
using System.Linq;

namespace SharpConnector.Configuration
{
    public class CouchbaseConfig : IConnectorConfig
    {
        public string ConnectionString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string BucketName { get; set; }

        // Not used
        public int DatabaseNumber { get; private set; }
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }
        public string AccessKey { get; private set; }
        public string SecretKey { get; private set; }
        public string Region { get; private set; }
        public string ServiceUrl { get; private set; }
        public bool UseHttp { get; private set; }
        public string TableName { get; private set; }

        public CouchbaseConfig(IConfiguration section)
        {
            var sectionChildren = section.GetChildren();
            var configurationSections = sectionChildren.ToList();

            var sectionChildrenConnectionString = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.connectionstring.ToString());
            ConnectionString = sectionChildrenConnectionString?.Value;

            var sectionChildrenBucketName = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.bucketname.ToString());
            BucketName = sectionChildrenBucketName?.Value.Trim();

            var sectionChildrenUsername = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.username.ToString());
            Username = sectionChildrenBucketName?.Value.Trim();

            var sectionChildrenPassword = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.password.ToString());
            Password = sectionChildrenPassword?.Value.Trim();
        }
    }
}
