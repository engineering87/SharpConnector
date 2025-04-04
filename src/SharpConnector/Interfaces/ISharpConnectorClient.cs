﻿// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpConnector.Interfaces
{
    /// <summary>
    /// Defines the operations for a SharpConnector client.
    /// </summary>
    /// <typeparam name="T">The payload type.</typeparam>
    public interface ISharpConnectorClient<T>
    {
        /// <summary>
        /// Retrieves an item by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>The item associated with the key.</returns>
        [Obsolete("Use GetAsync instead.")]
        T Get(string key);

        /// <summary>
        /// Asynchronously retrieves an item by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>A task containing the item associated with the key.</returns>
        Task<T> GetAsync(string key);

        /// <summary>
        /// Retrieves all items.
        /// </summary>
        /// <returns>An enumerable of all items.</returns>
        [Obsolete("Use GetAllAsync instead.")]
        IEnumerable<T> GetAll();

        /// <summary>
        /// Asynchronously retrieves all items.
        /// </summary>
        /// <returns>A task containing an enumerable of all items.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Inserts an item with a specified key.
        /// </summary>
        /// <param name="key">The unique key for the item.</param>
        /// <param name="value">The item to insert.</param>
        /// <returns>True if insertion was successful, false otherwise.</returns>
        [Obsolete("Use InsertAsync instead.")]
        bool Insert(string key, T value);

        /// <summary>
        /// Inserts an item with a specified key and expiration time.
        /// </summary>
        /// <param name="key">The unique key for the item.</param>
        /// <param name="value">The item to insert.</param>
        /// <param name="expiration">The time after which the item expires.</param>
        /// <returns>True if insertion was successful, false otherwise.</returns>
        [Obsolete("Use InsertAsync instead.")]
        bool Insert(string key, T value, TimeSpan expiration);

        /// <summary>
        /// Asynchronously inserts an item with a specified key.
        /// </summary>
        /// <param name="key">The unique key for the item.</param>
        /// <param name="value">The item to insert.</param>
        /// <returns>A task containing true if insertion was successful, false otherwise.</returns>
        Task<bool> InsertAsync(string key, T value);

        /// <summary>
        /// Asynchronously inserts an item with a specified key and expiration time.
        /// </summary>
        /// <param name="key">The unique key for the item.</param>
        /// <param name="value">The item to insert.</param>
        /// <param name="expiration">The time after which the item expires.</param>
        /// <returns>A task containing true if insertion was successful, false otherwise.</returns>
        Task<bool> InsertAsync(string key, T value, TimeSpan expiration);

        /// <summary>
        /// Inserts multiple items.
        /// </summary>
        /// <param name="values">A dictionary of items to insert, keyed by their unique keys.</param>
        /// <returns>True if all items were inserted successfully, false otherwise.</returns>
        [Obsolete("Use InsertManyAsync instead.")]
        bool InsertMany(Dictionary<string, T> values);

        /// <summary>
        /// Inserts multiple items with a specified expiration time.
        /// </summary>
        /// <param name="values">A dictionary of items to insert, keyed by their unique keys.</param>
        /// <param name="expiration">The time after which the items expire.</param>
        /// <returns>True if all items were inserted successfully, false otherwise.</returns>
        [Obsolete("Use InsertManyAsync instead.")]
        bool InsertMany(Dictionary<string, T> values, TimeSpan expiration);

        /// <summary>
        /// Asynchronously inserts multiple items.
        /// </summary>
        /// <param name="values">A collection of items to insert.</param>
        /// <returns>A task containing true if all items were inserted successfully, false otherwise.</returns>
        Task<bool> InsertManyAsync(IEnumerable<T> values);

        /// <summary>
        /// Deletes an item by its key.
        /// </summary>
        /// <param name="key">The unique key of the item to delete.</param>
        /// <returns>True if deletion was successful, false otherwise.</returns>
        [Obsolete("Use DeleteAsync instead.")]
        bool Delete(string key);

        /// <summary>
        /// Asynchronously deletes an item by its key.
        /// </summary>
        /// <param name="key">The unique key of the item to delete.</param>
        /// <returns>A task containing true if deletion was successful, false otherwise.</returns>
        Task<bool> DeleteAsync(string key);

        /// <summary>
        /// Updates an item by its key.
        /// </summary>
        /// <param name="key">The unique key of the item to update.</param>
        /// <param name="value">The new item value.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        [Obsolete("Use UpdateAsync instead.")]
        bool Update(string key, T value);

        /// <summary>
        /// Asynchronously updates an item by its key.
        /// </summary>
        /// <param name="key">The unique key of the item to update.</param>
        /// <param name="value">The new item value.</param>
        /// <returns>A task containing true if the update was successful, false otherwise.</returns>
        Task<bool> UpdateAsync(string key, T value);

        /// <summary>
        /// Checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        [Obsolete("Use ExistsAsync instead.")]
        bool Exists(string key);

        /// <summary>
        /// Asynchronously checks if an item exists by its key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>A task containing true if the item exists, false otherwise.</returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// Finds items that match a given filter expression.
        /// </summary>
        /// <param name="filter">A predicate function defining the search criteria.</param>
        /// <returns>A collection of items that match the filter.</returns>
        [Obsolete("Use QueryAsync instead.")]
        IEnumerable<T> Query(Func<T, bool> filter);

        /// <summary>
        /// Asynchronously finds items that match a given filter expression.
        /// </summary>
        /// <param name="filter">A predicate function defining the search criteria.</param>
        /// <returns>A task containing a collection of items that match the filter.</returns>
        Task<IEnumerable<T>> QueryAsync(Func<T, bool> filter);
    }
}