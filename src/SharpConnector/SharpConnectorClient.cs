// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
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

        public SharpConnectorClient(IConfiguration configuration)
        {
            InitOperations(configuration);
        }

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
        public async Task<T> GetAsync(string key)
        {
            return await _operations.GetAsync(key);
        }

        /// <summary>
        /// Get all the values.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            return _operations.GetAll();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _operations.GetAllAsync();
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
        /// <param name="expiration">The expiration of the key.</param>
        /// <returns></returns>
        public bool Insert(string key, T value, TimeSpan expiration)
        {
            return _operations.Insert(key, value, expiration);
        }

        /// <summary>
        /// Set key to hold the object value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(string key, T value)
        {
            return await _operations.InsertAsync(key, value);
        }

        /// <summary>
        /// Set key to hold the object value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the object.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiration">The expiration of the key.</param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(string key, T value, TimeSpan expiration)
        {
            return await _operations.InsertAsync(key, value, expiration);
        }

        /// <summary>
        /// Multiple set keys to hold the object values.
        /// </summary>
        /// <param name="values">The <key, object> dictionary to set.</param>
        /// <returns></returns>
        public bool InsertMany(Dictionary<string, T> values)
        {
            return _operations.InsertMany(values);
        }

        /// <summary>
        /// Multiple set keys to hold the object values.
        /// </summary>
        /// <param name="values">The <key, object> dictionary to set.</param>
        /// <param name="expiration">The expiration of the keys.</param>
        /// <returns></returns>
        public bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _operations.InsertMany(values, expiration);
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
        public async Task<bool> DeleteAsync(string key)
        {
            return await _operations.DeleteAsync(key);
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
        public async Task<bool> UpdateAsync(string key, T value)
        {
            return await _operations.UpdateAsync(key, value);
        }

        /// <summary>
        /// Asynchronously inserts multiple ConnectorEntity instances into the storage.
        /// </summary>
        /// <param name="values">A list of ConnectorEntity instances to be inserted.</param>
        /// <returns>A Task representing the asynchronous operation, containing a boolean value indicating the success or failure of the insert operation.</returns>
        public async Task<bool> InsertManyAsync(IEnumerable<T> values)
        {
            return await _operations.InsertManyAsync(values);
        }
    }
}
