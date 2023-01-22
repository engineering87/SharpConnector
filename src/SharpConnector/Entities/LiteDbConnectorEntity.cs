// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using LiteDB;

namespace SharpConnector.Entities
{
    [Serializable]
    public class LiteDbConnectorEntity : ConnectorEntity
    {
        public LiteDbConnectorEntity() { }

        [BsonCtor]
        public LiteDbConnectorEntity(string key, object payload, TimeSpan? expiration) 
            : base(key, payload, expiration)
        {

        }
    }
}