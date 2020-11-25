// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Newtonsoft.Json;
using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SharpConnector.Entities
{
    /// <summary>
    /// This class incapsulate Key and Payload.
    /// </summary>
    [Serializable]
    [BsonIgnoreExtraElements]
    public class ConnectorEntity
    {
        public string Key { get; set; }
        public object Payload { get; set; }
        public TimeSpan? Expiration { get; set; }

        /// <summary>
        /// Empty constructor for serialization.
        /// </summary>
        public ConnectorEntity() { }

        /// <summary>
        /// Constructor with expiration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="payload"></param>
        /// <param name="expiration"></param>
        public ConnectorEntity(string key, object payload, TimeSpan? expiration)
        {
            if (string.IsNullOrEmpty(key) || payload == null)
                throw new ArgumentException("Key or Payload cannot be null");

            var type = payload.GetType();
            if (type.IsSerializable)
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
        /// Constructor with no expiration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="payload"></param>
        public ConnectorEntity(string key, object payload)
        {
            if (string.IsNullOrEmpty(key) || payload == null)
                throw new ArgumentException("Key or Payload cannot be null");

            var type = payload.GetType();
            if (type.IsSerializable)
            {
                Key = key;
                Payload = payload;
                Expiration = null;
            }
            else
            {
                throw new ArgumentException("Payload cannot be serialized");
            }
        }

        /// <summary>
        /// Equals override method.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ConnectorEntity p))
                return false;

            return Payload.GetHashCode() == p.GetHashCode();
        }

        /// <summary>
        /// Hashcode override method.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Payload.GetHashCode() ^ Expiration.GetHashCode();
        }

        /// <summary>
        /// ToString override method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
