// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using SharpConnector.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConnector.Operations
{
    /// <summary>
    /// Operations for handling CRUD operations on a specific payload type.
    /// </summary>
    /// <typeparam name="T">Payload object type.</typeparam>
    public abstract class Operations<T> : IOperations<T>
    {
        /// <inheritdoc />
        public abstract T Get(string key);

        /// <inheritdoc />
        public abstract Task<T> GetAsync(string key, CancellationToken ct = default);

        /// <inheritdoc />
        public abstract IEnumerable<T> GetAll();

        /// <inheritdoc />
        public abstract Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);

        /// <inheritdoc />
        public abstract bool Insert(string key, T value);

        /// <inheritdoc />
        public abstract bool Insert(string key, T value, TimeSpan expiration);

        /// <inheritdoc />
        public abstract Task<bool> InsertAsync(string key, T value, CancellationToken ct = default);

        /// <inheritdoc />
        public abstract Task<bool> InsertAsync(string key, T value, TimeSpan expiration, CancellationToken ct = default);

        /// <inheritdoc />
        public abstract bool InsertMany(Dictionary<string, T> values);

        /// <inheritdoc />
        public abstract bool InsertMany(Dictionary<string, T> values, TimeSpan expiration);

        /// <inheritdoc />
        public abstract Task<bool> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default);

        /// <inheritdoc />
        public abstract bool Delete(string key);

        /// <inheritdoc />
        public abstract Task<bool> DeleteAsync(string key, CancellationToken ct = default);

        /// <inheritdoc />
        public abstract bool Update(string key, T value);

        /// <inheritdoc />
        public abstract Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default);

        /// <inheritdoc />
        public abstract bool Exists(string key);

        /// <inheritdoc />
        public abstract Task<bool> ExistsAsync(string key, CancellationToken ct = default);

        /// <inheritdoc />
        public abstract IEnumerable<T> Query(Func<T, bool> filter);

        /// <inheritdoc />
        public abstract Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter, CancellationToken ct = default);
    }
}