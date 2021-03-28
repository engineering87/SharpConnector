using System;
using LiteDB;

namespace SharpConnector.Entities
{
    [Serializable]
    public class LiteDbConnectorEntity : ConnectorEntity
    {
        //[BsonId]
        //[BsonField("_id")]
        //public ObjectId Id { get; } = ObjectId.NewObjectId();

        public LiteDbConnectorEntity() { }

        [BsonCtor]
        public LiteDbConnectorEntity(string key, object payload, TimeSpan? expiration) 
            : base(key, payload, expiration)
        {

        }
    }
}