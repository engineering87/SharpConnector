// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Newtonsoft.Json;
using System;

namespace SharpConnector.Entities
{
    /// <summary>
    /// An ArangoDB-specific wrapper for ConnectorEntity that maps <see cref="ConnectorEntity.Key"/>
    /// to the ArangoDB document key (<c>_key</c>), so documents can be addressed by Key.
    /// </summary>
    public class ArangoConnectorEntity : ConnectorEntity
    {
        public ArangoConnectorEntity() { }

        public ArangoConnectorEntity(ConnectorEntity source)
        {
            ArgumentNullException.ThrowIfNull(source);
            Key = source.Key;
            Payload = source.Payload;
            Expiration = source.Expiration;
            ExpiresAt = source.ExpiresAt;
        }

        /// <summary>
        /// The ArangoDB document key, always equal to <see cref="ConnectorEntity.Key"/>.
        /// </summary>
        [JsonProperty("_key")]
        public string DocumentKey
        {
            get => Key;
            set => Key ??= value;
        }
    }
}
