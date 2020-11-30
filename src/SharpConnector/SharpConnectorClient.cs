// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Interfaces;
using SharpConnector.Operations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpConnector
{
    public sealed class SharpConnectorClient<T> : ISharpConnectorClient<T>
    {
        private IOperations<T> _operations;

        /// <summary>
        /// Create e new SharpConnectorClient instance.
        /// </summary>
        public SharpConnectorClient()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            InitOperations(configuration);
        }

        /// <summary>
        /// Create e new SharpConnectorClient instance.
        /// </summary>
        /// <param name="builder">The configuration builder.</param>
        public SharpConnectorClient(IConfigurationBuilder builder)
        {
            var configuration = builder.Build();

            InitOperations(configuration);
        }

        /// <summary>
        /// Create e new SharpConnectorClient instance.
        /// </summary>
        /// <param name="jsonConfigFileName">The config file name.</param>
        public SharpConnectorClient(string jsonConfigFileName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile(jsonConfigFileName, optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            InitOperations(configuration);
        }

        /// <summary>
        /// Init the Operations instance related to the connector type and config.
        /// </summary>
        /// <param name="configurationSection"></param>
        private void InitOperations(IConfiguration configurationSection)
        {
            var section = configurationSection.GetSection("ConnectorConfig");
            if (section == null)
            {
                throw new KeyNotFoundException("ConnectorConfig for SharpConnector was not found.");
            }

            _operations = new OperationsFactory<T>(section).GetStrategy();
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
