// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpConnector.Interfaces
{
    /// <summary>
    /// Operations interface for handling CRUD operations on a specific payload type.
    /// </summary>
    /// <typeparam name="T">Payload object type.</typeparam>
    public interface IOperations<T>
    {
        /// <summary>
        /// Retrieves a single item by its key.
        /// </summary>
        /// <param name="key">The key of the item to retrieve.</param>
        /// <returns>The payload object associated with the provided key.</returns>
        T Get(string key);

        /// <summary>
        /// Asynchronously retrieves a single item by its key.
        /// </summary>
        /// <param name="key">The key of the item to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation, with the result being the payload object.</returns>
        Task<T> GetAsync(string key);

        /// <summary>
        /// Retrieves all items.
        /// </summary>
        /// <returns>A collection of all payload objects.</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Asynchronously retrieves all items.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, with the result being a collection of all payload objects.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Inserts a new item with a specified key and value.
        /// </summary>
        /// <param name="key">The key of the item to insert.</param>
        /// <param name="value">The value (payload) of the item to insert.</param>
        /// <returns>True if the insertion was successful, otherwise false.</returns>
        bool Insert(string key, T value);

        /// <summary>
        /// Inserts a new item with a specified key, value, and expiration time.
        /// </summary>
        /// <param name="key">The key of the item to insert.</param>
        /// <param name="value">The value (payload) of the item to insert.</param>
        /// <param name="expiration">The expiration time for the item.</param>
        /// <returns>True if the insertion was successful, otherwise false.</returns>
        bool Insert(string key, T value, TimeSpan expiration);

        /// <summary>
        /// Asynchronously inserts a new item with a specified key and value.
        /// </summary>
        /// <param name="key">The key of the item to insert.</param>
        /// <param name="value">The value (payload) of the item to insert.</param>
        /// <returns>A task that represents the asynchronous operation, with the result being true if successful.</returns>
        Task<bool> InsertAsync(string key, T value);

        /// <summary>
        /// Asynchronously inserts a new item with a specified key, value, and expiration time.
        /// </summary>
        /// <param name="key">The key of the item to insert.</param>
        /// <param name="value">The value (payload) of the item to insert.</param>
        /// <param name="expiration">The expiration time for the item.</param>
        /// <returns>A task that represents the asynchronous operation, with the result being true if successful.</returns>
        Task<bool> InsertAsync(string key, T value, TimeSpan expiration);

        /// <summary>
        /// Inserts multiple items at once.
        /// </summary>
        /// <param name="values">A dictionary containing keys and their corresponding payloads to insert.</param>
        /// <returns>True if all insertions were successful, otherwise false.</returns>
        bool InsertMany(Dictionary<string, T> values);

        /// <summary>
        /// Inserts multiple items at once with an expiration time.
        /// </summary>
        /// <param name="values">A dictionary containing keys and their corresponding payloads to insert.</param>
        /// <param name="expiration">The expiration time for the items.</param>
        /// <returns>True if all insertions were successful, otherwise false.</returns>
        bool InsertMany(Dictionary<string, T> values, TimeSpan expiration);

        /// <summary>
        /// Asynchronously inserts multiple items at once.
        /// </summary>
        /// <param name="values">A collection of payload objects to insert.</param>
        /// <returns>A task that represents the asynchronous operation, with the result being true if all insertions were successful.</returns>
        Task<bool> InsertManyAsync(IEnumerable<T> values);

        /// <summary>
        /// Deletes an item by its key.
        /// </summary>
        /// <param name="key">The key of the item to delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        bool Delete(string key);

        /// <summary>
        /// Asynchronously deletes an item by its key.
        /// </summary>
        /// <param name="key">The key of the item to delete.</param>
        /// <returns>A task that represents the asynchronous operation, with the result being true if successful.</returns>
        Task<bool> DeleteAsync(string key);

        /// <summary>
        /// Updates an existing item with a new value.
        /// </summary>
        /// <param name="key">The key of the item to update.</param>
        /// <param name="value">The new value to set for the item.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        bool Update(string key, T value);

        /// <summary>
        /// Asynchronously updates an existing item with a new value.
        /// </summary>
        /// <param name="key">The key of the item to update.</param>
        /// <param name="value">The new value to set for the item.</param>
        /// <returns>A task that represents the asynchronous operation, with the result being true if successful.</returns>
        Task<bool> UpdateAsync(string key, T value);

        /// <summary>
        /// Checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        bool Exists(string key);

        /// <summary>
        /// Asynchronously checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>A task containing true if the item exists, false otherwise.</returns>
        Task<bool> ExistsAsync(string key);
    }
}
