// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Text.Json;

namespace SharpConnector.Utilities
{
    public static class Serialization
    {
        /// <summary>
        /// Determines if the given object is serializable.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object can be serialized; otherwise, false.</returns>
        public static bool IsSerializable(this object obj)
        {
            try
            {
                // Attempt to serialize the object
                JsonSerializer.Serialize(obj);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
