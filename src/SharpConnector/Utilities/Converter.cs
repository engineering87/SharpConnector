// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace SharpConnector.Utilities
{
    public static class Converter
    {
        /// <summary>
        /// Convert the ConnectorEntity object to original object.
        /// </summary>
        /// <typeparam name="T">The original object stored.</typeparam>
        /// <param name="connectorEntity">The ConnectorEntity object to convert.</param>
        /// <returns></returns>
        public static T ToPayloadObject<T>(this ConnectorEntity connectorEntity)
        {
            if (connectorEntity == null)
                return default;
            return ConvertPayload<T>(connectorEntity.Payload);
        }

        /// <summary>
        /// Convert the LiteDbConnectorEntity object to original object.
        /// </summary>
        /// <typeparam name="T">The original object stored.</typeparam>
        /// <param name="liteDbConnectorEntity">The LiteDbConnectorEntity object to convert.</param>
        /// <returns></returns>
        public static T ToPayloadObject<T>(this LiteDbConnectorEntity liteDbConnectorEntity)
        {
            if (liteDbConnectorEntity == null)
                return default;
            return ConvertPayload<T>(liteDbConnectorEntity.Payload);
        }

        /// <summary>
        /// Convert the ConnectorEntity list to original object list.
        /// </summary>
        /// <typeparam name="T">The original object stored.</typeparam>
        /// <param name="connectorEntities">The ConnectorEntity list to convert.</param>
        /// <returns></returns>
        public static IEnumerable<T> ToPayloadList<T>(this IEnumerable<ConnectorEntity> connectorEntities)
        {
            var result = new List<T>();
            foreach (var entity in connectorEntities)
            {
                if (entity.Payload != null)
                    result.Add(ConvertPayload<T>(entity.Payload));
            }
            return result;
        }

        /// <summary>
        /// Convert the LiteDbConnectorEntity list to original object list.
        /// </summary>
        /// <typeparam name="T">The original object stored.</typeparam>
        /// <param name="connectorEntities">The LiteDbConnectorEntity list to convert.</param>
        /// <returns></returns>
        public static IEnumerable<T> ToPayloadList<T>(this IEnumerable<LiteDbConnectorEntity> connectorEntities)
        {
            var result = new List<T>();
            foreach (var entity in connectorEntities)
            {
                if (entity.Payload != null)
                    result.Add(ConvertPayload<T>(entity.Payload));
            }
            return result;
        }

        /// <summary>
        /// Converts a payload object to the target type, handling JObject/JArray from JSON deserialization.
        /// </summary>
        private static T ConvertPayload<T>(object payload)
        {
            if (payload is null)
                return default;

            if (payload is T typed)
                return typed;

            if (payload is JToken jToken)
                return jToken.ToObject<T>();

            var targetType = typeof(T);

            // Fast path for primitives / IConvertible target types
            if (payload is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType))
            {
                try
                {
                    return (T)Convert.ChangeType(payload, targetType);
                }
                catch (InvalidCastException) { /* fallback below */ }
                catch (FormatException) { /* fallback below */ }
            }

            // Fallback: round-trip via JSON for arbitrary POCOs (e.g. BSON documents from MongoDB)
            var json = JsonConvert.SerializeObject(payload);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Returns a list of ConnectorEntity.
        /// </summary>
        /// <typeparam name="T">The generic type to store.</typeparam>
        /// <param name="values">The dictionary with <key, object> elements to store.</param>
        /// <returns></returns>
        public static List<ConnectorEntity> ToConnectorEntityList<T>(this Dictionary<string, T> values, TimeSpan? expiration = null)
        {
            if (values == null) 
                return [];

            var connectorEntities = new List<ConnectorEntity>();
            foreach (var entry in values)
            {
                if (string.IsNullOrEmpty(entry.Key) || entry.Value == null)
                    continue;
                connectorEntities.Add(new ConnectorEntity(entry.Key, entry.Value, expiration));
            }

            return connectorEntities;
        }

        /// <summary>
        /// Returns a list of LiteDbConnectorEntity.
        /// </summary>
        /// <typeparam name="T">The generic type to store.</typeparam>
        /// <param name="values">The dictionary with <key, object> elements to store.</param>
        /// <returns></returns>
        public static List<LiteDbConnectorEntity> ToLiteDbConnectorEntityList<T>(this Dictionary<string, T> values, TimeSpan? expiration = null)
        {
            if (values == null)
                return [];

            var connectorEntities = new List<LiteDbConnectorEntity>();
            foreach (var entry in values)
            {
                if (string.IsNullOrEmpty(entry.Key) || entry.Value == null)
                    continue;
                connectorEntities.Add(new LiteDbConnectorEntity(entry.Key, entry.Value, expiration));
            }

            return connectorEntities;
        }

        /// <summary>
        /// Returns a list of MongoConnectorEntity.
        /// </summary>
        /// <typeparam name="T">The generic type to store.</typeparam>
        /// <param name="values">The dictionary with <key, object> elements to store.</param>
        /// <returns></returns>
        public static List<MongoConnectorEntity> ToMongoDbConnectorEntityList<T>(this Dictionary<string, T> values, TimeSpan? expiration = null)
        {
            if (values == null)
                return [];

            var connectorEntities = new List<MongoConnectorEntity>();
            foreach (var entry in values)
            {
                if (string.IsNullOrEmpty(entry.Key) || entry.Value == null)
                    continue;
                connectorEntities.Add(new MongoConnectorEntity(entry.Key, entry.Value, expiration));
            }

            return connectorEntities;
        }
    }
}
