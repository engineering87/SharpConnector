// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Configuration;
using SharpConnector.Operations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpConnector
{
    public sealed class SharpConnectorClient<T>
    {
        private readonly IOperations<T> _operations;

        /// <summary>
        /// Create e new SharpConnectorClient instance.
        /// </summary>
        /// <param name="connectorTypes">The connector type.</param>
        public SharpConnectorClient(ConnectorTypes connectorTypes)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            var section = configuration.GetSection("ConnectorConfig");
            if (section == null)
            {
                throw new KeyNotFoundException("Environment variable for SharpConnector was not found.");
            }

            var connectorConfig = new ConfigFactory().GetConfigurationStrategy(section, connectorTypes);
            _operations = new OperationsFactory<T>().GetOperationsStrategy(connectorTypes, connectorConfig);
        }

        /// <summary>
        /// Create e new SharpConnectorClient instance.
        /// </summary>
        /// <param name="connectorTypes">The connector type.</param>
        /// <param name="builder">The configuration builder.</param>
        public SharpConnectorClient(ConnectorTypes connectorTypes, IConfigurationBuilder builder)
        {
            var configuration = builder.Build();
            var section = configuration.GetSection("ConnectorConfig");
            if (section == null)
            {
                throw new KeyNotFoundException("Environment variable for SharpConnector was not found.");
            }

            var connectorConfig = new ConfigFactory().GetConfigurationStrategy(section, connectorTypes);
            _operations = new OperationsFactory<T>().GetOperationsStrategy(connectorTypes, connectorConfig);
        }

        /// <summary>
        /// Create e new SharpConnectorClient instance.
        /// </summary>
        /// <param name="connectorTypes">The connector type.</param>
        /// <param name="jsonConfigFileName">The config file name.</param>
        public SharpConnectorClient(ConnectorTypes connectorTypes, string jsonConfigFileName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile(jsonConfigFileName, optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            var section = configuration.GetSection("ConnectorConfig");
            if (section == null)
            {
                throw new KeyNotFoundException("Environment variable for SharpConnector was not found.");
            }

            var connectorConfig = new ConfigFactory().GetConfigurationStrategy(section, connectorTypes);
            _operations = new OperationsFactory<T>().GetOperationsStrategy(connectorTypes, connectorConfig);
        }

        /// <summary>
        /// Get the value of key. If the key does not exist the default value is returned.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public T Get(string key)
        {
            return _operations.Get(key);
        }

        /// <summary>
        /// Get the value of key. If the key does not exist the default value is returned.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public Task<T> GetAsync(string key)
        {
            return _operations.GetAsync(key);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public bool Insert(string key, T value)
        {
            return _operations.Insert(key, value);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public Task<bool> InsertAsync(string key, T value)
        {
            return _operations.InsertAsync(key, value);
        }

        /// <summary>
        /// Removes the specified key. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            return _operations.Delete(key);
        }

        /// <summary>
        /// Removes the specified key. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(string key)
        {
            return _operations.DeleteAsync(key);
        }

        /// <summary>
        /// Update key to hold the string value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public bool Update(string key, T value)
        {
            return _operations.Update(key, value);
        }

        /// <summary>
        /// Update key to hold the string value.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(string key, T value)
        {
            return _operations.UpdateAsync(key, value);
        }
    }
}
