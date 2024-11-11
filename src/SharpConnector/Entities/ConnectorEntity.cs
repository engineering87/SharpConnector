// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Newtonsoft.Json;
using System;
using MongoDB.Bson.Serialization.Attributes;
using SharpConnector.Utilities;

namespace SharpConnector.Entities
{
    /// <summary>
    /// Represents a data entity containing a key, payload, and an optional expiration time.
    /// This class encapsulates a key-value pair with optional time-based expiration.
    /// </summary>
    [Serializable]
    [BsonIgnoreExtraElements]
    public class ConnectorEntity
    {
        /// <summary>
        /// The key associated with the payload.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value (payload) associated with the key. This can be any serializable object.
        /// </summary>
        public object Payload { get; set; }

        /// <summary>
        /// The optional expiration time for the key-value pair.
        /// If not provided, the data does not expire.
        /// </summary>
        public TimeSpan? Expiration { get; set; }

        /// <summary>
        /// Default constructor for serialization purposes.
        /// </summary>
        public ConnectorEntity() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorEntity"/> class with a key, payload, and optional expiration time.
        /// </summary>
        /// <param name="key">The key associated with the payload.</param>
        /// <param name="payload">The payload to be stored with the key.</param>
        /// <param name="expiration">The optional expiration time for the key-value pair.</param>
        /// <exception cref="ArgumentException">Thrown if the key or payload is null, or if the payload is not serializable.</exception>
        public ConnectorEntity(string key, object payload, TimeSpan? expiration)
        {
            if (string.IsNullOrEmpty(key) || payload == null)
                throw new ArgumentException("Key or Payload cannot be null");

            if (payload.IsSerializable())  // Check if the payload is serializable
            {
                Key = key;
                Payload = payload;
                Expiration = expiration;
            }
            else
            {
                throw new ArgumentException("Payload cannot be serialized");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorEntity"/> class with a key and payload, without an expiration time.
        /// </summary>
        /// <param name="key">The key associated with the payload.</param>
        /// <param name="payload">The payload to be stored with the key.</param>
        /// <exception cref="ArgumentException">Thrown if the key or payload is null, or if the payload is not serializable.</exception>
        public ConnectorEntity(string key, object payload)
        {
            if (string.IsNullOrEmpty(key) || payload == null)
                throw new ArgumentException("Key or Payload cannot be null");

            if (payload.IsSerializable())  // Check if the payload is serializable
            {
                Key = key;
                Payload = payload;
                Expiration = null;  // No expiration time specified
            }
            else
            {
                throw new ArgumentException("Payload cannot be serialized");
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current ConnectorEntity instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current ConnectorEntity instance.</param>
        /// <returns>True if the specified object is a ConnectorEntity and has the same Payload hash code; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is not ConnectorEntity p)
                return false;

            return Payload.GetHashCode() == p.GetHashCode();
        }

        /// <summary>
        /// Serves as the hash function for the ConnectorEntity type.
        /// </summary>
        /// <returns>A hash code for the current instance, combining the hash codes of the Key, Payload, and Expiration.</returns>
        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Payload.GetHashCode() ^ Expiration.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the current ConnectorEntity instance.
        /// </summary>
        /// <returns>A JSON string representing the current instance.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
