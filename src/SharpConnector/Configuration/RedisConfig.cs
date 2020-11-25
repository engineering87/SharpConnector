// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace SharpConnector.Configuration
{
    /// <summary>
    /// Redis configuration class.
    /// </summary>
    public class RedisConfig : ConnectorConfig
    {
        public int DatabaseNumber { get; set; }
    }
}
