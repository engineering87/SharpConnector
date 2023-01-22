// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpConnector.Interfaces
{
    /// <summary>
    /// The SharpConnectorClient operations interface.
    /// </summary>
    /// <typeparam name="T">The payload type.</typeparam>
    public interface ISharpConnectorClient<T>
    {
        T Get(string key);
        Task<T> GetAsync(string key);
        IEnumerable<T> GetAll();
        bool Insert(string key, T value);
        bool Insert(string key, T value, TimeSpan expiration);
        Task<bool> InsertAsync(string key, T value);
        Task<bool> InsertAsync(string key, T value, TimeSpan expiration);
        bool InsertMany(Dictionary<string, T> values);
        bool InsertMany(Dictionary<string, T> values, TimeSpan expiration);
        bool Delete(string key);
        Task<bool> DeleteAsync(string key);
        bool Update(string key, T value);
        Task<bool> UpdateAsync(string key, T value);
    }
}