// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace SharpConnector.Interfaces
{
    public interface IConnectorConfig
    {
        int DatabaseNumber { get; }
        string ConnectionString { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
    }
}
