// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Threading.Tasks;

namespace SharpConnector.Operations
{
    /// <summary>
    /// Operations abstract class.
    /// </summary>
    /// <typeparam name="T">The Payload object type.</typeparam>
    public abstract class Operations<T> : IOperations<T>
    {
        public abstract T Get(string key);
        public abstract Task<T> GetAsync(string key);
        public abstract bool Insert(string key, T value);
        public abstract Task<bool> InsertAsync(string key, T value);
        public abstract bool Delete(string key);
        public abstract Task<bool> DeleteAsync(string key);
        public abstract bool Update(string key, T value);
        public abstract Task<bool> UpdateAsync(string key, T value);
    }
}
