// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using SharpConnector.Interfaces;
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
        public abstract Task<T> GetAsync(string key);
        /// <inheritdoc />
        public abstract IEnumerable<T> GetAll();
        /// <inheritdoc />
        public abstract Task<IEnumerable<T>> GetAllAsync();
        /// <inheritdoc />
        public abstract bool Insert(string key, T value);
        /// <inheritdoc />
        public abstract bool Insert(string key, T value, TimeSpan expiration);
        /// <inheritdoc />
        public abstract Task<bool> InsertAsync(string key, T value);
        /// <inheritdoc />
        public abstract Task<bool> InsertAsync(string key, T value, TimeSpan expiration);
        /// <inheritdoc />
        public abstract bool InsertMany(Dictionary<string, T> values);
        /// <inheritdoc />
        public abstract bool InsertMany(Dictionary<string, T> values, TimeSpan expiration);
        /// <inheritdoc />
        public abstract Task<bool> InsertManyAsync(IEnumerable<T> values);
        /// <inheritdoc />
        public abstract bool Delete(string key);
        /// <inheritdoc />
        public abstract Task<bool> DeleteAsync(string key);
        /// <inheritdoc />
        public abstract bool Update(string key, T value);
        /// <inheritdoc />
        public abstract Task<bool> UpdateAsync(string key, T value);
    }
}
