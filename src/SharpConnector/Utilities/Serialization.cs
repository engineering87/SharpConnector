// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using Newtonsoft.Json;

namespace SharpConnector.Utilities
{
    public static class Serialization
    {
        /// <summary>
        /// Determines if the given object is serializable using the same
        /// serializer used by the connectors (Newtonsoft.Json).
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object can be serialized; otherwise, false.</returns>
        public static bool IsSerializable(this object obj)
        {
            if (obj is null)
                return true;

            try
            {
                JsonConvert.SerializeObject(obj);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
