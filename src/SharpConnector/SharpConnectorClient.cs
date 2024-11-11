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

        /// <inheritdoc />
        public T Get(string key)
        {
            return _operations.Get(key);
        }

        /// <inheritdoc />
        public async Task<T> GetAsync(string key)
        {
            return await _operations.GetAsync(key);
        }

        /// <inheritdoc />
        public IEnumerable<T> GetAll()
        {
            return _operations.GetAll();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _operations.GetAllAsync();
        }

        /// <inheritdoc />
        public bool Insert(string key, T value)
        {
            return _operations.Insert(key, value);
        }

        /// <inheritdoc />
        public bool Insert(string key, T value, TimeSpan expiration)
        {
            return _operations.Insert(key, value, expiration);
        }

        /// <inheritdoc />
        public async Task<bool> InsertAsync(string key, T value)
        {
            return await _operations.InsertAsync(key, value);
        }

        /// <inheritdoc />
        public async Task<bool> InsertAsync(string key, T value, TimeSpan expiration)
        {
            return await _operations.InsertAsync(key, value, expiration);
        }

        /// <inheritdoc />
        public bool InsertMany(Dictionary<string, T> values)
        {
            return _operations.InsertMany(values);
        }

        /// <inheritdoc />
        public bool InsertMany(Dictionary<string, T> values, TimeSpan expiration)
        {
            return _operations.InsertMany(values, expiration);
        }

        /// <inheritdoc />
        public bool Delete(string key)
        {
            return _operations.Delete(key);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(string key)
        {
            return await _operations.DeleteAsync(key);
        }

        /// <inheritdoc />
        public bool Update(string key, T value)
        {
            return _operations.Update(key, value);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAsync(string key, T value)
        {
            return await _operations.UpdateAsync(key, value);
        }

        /// <inheritdoc />
        public async Task<bool> InsertManyAsync(IEnumerable<T> values)
        {
            return await _operations.InsertManyAsync(values);
        }
    }
}
