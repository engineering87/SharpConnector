// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace SharpConnector.Interfaces
{
    /// <summary>
    /// Configuration interface.
    /// </summary>
    public interface IConnectorConfig
    {
        int DatabaseNumber { get; }
        string ConnectionString { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
        public string Username { get; }
        public string Password { get; }
        public string BucketName { get; }
        public string AccessKey { get; }
        public string SecretKey { get; }
        public string Region { get; }
        public string ServiceUrl { get; }
        public bool UseHttp { get; }
        public string TableName { get; }
    }
}
