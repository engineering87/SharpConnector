// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace SharpConnector.Configuration
{
    /// <summary>
    /// MongoDb configuration class.
    /// </summary>
    public class MongoDbConfig : ConnectorConfig
    {
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}
