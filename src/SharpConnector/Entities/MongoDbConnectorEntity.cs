using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SharpConnector.Entities
{
    /// <summary>
    /// A MongoDB-specific wrapper for ConnectorEntity to include BSON annotations.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class MongoConnectorEntity : ConnectorEntity
    {
        // Inherits all properties from ConnectorEntity and includes BSON-specific behavior.

        public MongoConnectorEntity() { }

        public MongoConnectorEntity(string key, object payload, TimeSpan? expiration)
            : base(key, payload, expiration)
        {

        }
    }
}