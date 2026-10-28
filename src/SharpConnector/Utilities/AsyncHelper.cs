// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Threading.Tasks;

namespace SharpConnector.Utilities
{
    /// <summary>
    /// Runs asynchronous delegates synchronously on the thread pool, so that a captured
    /// SynchronizationContext cannot cause a deadlock when blocking on the result.
    /// </summary>
    internal static class AsyncHelper
    {
        /// <summary>
        /// Executes the asynchronous function and blocks until it completes, returning its result.
        /// </summary>
        public static T RunSync<T>(Func<Task<T>> func)
        {
            ArgumentNullException.ThrowIfNull(func);
            return Task.Run(func).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes the asynchronous function and blocks until it completes.
        /// </summary>
        public static void RunSync(Func<Task> func)
        {
            ArgumentNullException.ThrowIfNull(func);
            Task.Run(func).GetAwaiter().GetResult();
        }
    }
}
