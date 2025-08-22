// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using SharpConnector.Interfaces;
using SharpConnector.Operations;
using System;
using System.Collections.Generic;
using System.Threading;
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
            ArgumentNullException.ThrowIfNull(builder);

            var configuration = builder.Build();

            InitOperations(configuration);
        }

        /// <summary>
        /// Create e new SharpConnectorClient instance.
        /// </summary>
        /// <param name="jsonConfigFileName">The config file name.</param>
        public SharpConnectorClient(string jsonConfigFileName)
        {
            if (string.IsNullOrWhiteSpace(jsonConfigFileName))
                throw new ArgumentException("Config file name cannot be null or empty.", nameof(jsonConfigFileName));

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
            var section = configurationSection?.GetSection("ConnectorConfig");
            if (section == null || !section.Exists())
                throw new KeyNotFoundException("ConnectorConfig for SharpConnector was not found.");

            _operations = new OperationsFactory<T>(section).GetStrategy();
        }

        /// <inheritdoc />
        public T Get(string key)
        {
            return _operations.Get(key);
        }

        /// <inheritdoc />
        public async Task<T> GetAsync(string key, CancellationToken ct = default)
        {
            return await _operations.GetAsync(key, ct);
        }

        /// <inheritdoc />
        public IEnumerable<T> GetAll()
        {
            return _operations.GetAll();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            return await _operations.GetAllAsync(ct);
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
        public async Task<bool> InsertAsync(string key, T value, CancellationToken ct = default)
        {
            return await _operations.InsertAsync(key, value, ct);
        }

        /// <inheritdoc />
        public async Task<bool> InsertAsync(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        {
            return await _operations.InsertAsync(key, value, expiration, ct);
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
        public async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            return await _operations.DeleteAsync(key, ct);
        }

        /// <inheritdoc />
        public bool Update(string key, T value)
        {
            return _operations.Update(key, value);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default)
        {
            return await _operations.UpdateAsync(key, value, ct);
        }

        /// <inheritdoc />
        public async Task<bool> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default)
        {
            return await _operations.InsertManyAsync(values, ct);
        }

        /// <inheritdoc />
        public bool Exists(string key)
        {
            return _operations.Exists(key);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            return await _operations.ExistsAsync(key, ct);
        }

        /// <inheritdoc />
        public IEnumerable<T> Query(Func<T, bool> filter)
        {
            return _operations.Query(filter);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter, CancellationToken ct = default)
        {
            return await (_operations.QueryAsync(filter, ct));
        }
    }
}
