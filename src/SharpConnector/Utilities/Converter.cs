// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace SharpConnector.Utilities
{
    /// <summary>
    /// Conversion helpers between user payloads and the connector entities persisted by each backend.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Convert the ConnectorEntity object to original object.
        /// </summary>
        /// <typeparam name="T">The original object stored.</typeparam>
        /// <param name="connectorEntity">The ConnectorEntity object to convert.</param>
        /// <returns>The payload converted to <typeparamref name="T"/>, or <c>default</c> if the entity is null.</returns>
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
        /// <returns>The payload converted to <typeparamref name="T"/>, or <c>default</c> if the entity is null.</returns>
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
        /// <returns>The converted payloads; entities with a null payload are skipped.</returns>
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
        /// <returns>The converted payloads; entities with a null payload are skipped.</returns>
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
        /// Converts a dictionary of key/payload pairs into a list of ConnectorEntity.
        /// </summary>
        /// <typeparam name="T">The generic type to store.</typeparam>
        /// <param name="values">The dictionary of key/payload pairs to store. A null dictionary yields an empty list.</param>
        /// <param name="expiration">Optional expiration applied to every entity.</param>
        /// <returns>The list of entities, in dictionary enumeration order.</returns>
        /// <exception
        public static List<ConnectorEntity> ToConnectorEntityList<T>(this Dictionary<string, T> values, TimeSpan? expiration = null)
        {
            if (values == null) 
                return [];

            var connectorEntities = new List<ConnectorEntity>();
            foreach (var entry in values)
            {
                if (string.IsNullOrEmpty(entry.Key))
                    throw new ArgumentException("Keys cannot be null or empty.", nameof(values));
                if (entry.Value == null)
                    throw new ArgumentException($"Value for key '{entry.Key}' cannot be null.", nameof(values));
                connectorEntities.Add(new ConnectorEntity(entry.Key, entry.Value, expiration));
            }

            return connectorEntities;
        }

        /// <summary>
        /// Converts a dictionary of key/payload pairs into a list of LiteDbConnectorEntity.
        /// </summary>
        /// <typeparam name="T">The generic type to store.</typeparam>
        /// <param name="values">The dictionary of key/payload pairs to store. A null dictionary yields an empty list.</param>
        /// <param name="expiration">Optional expiration applied to every entity.</param>
        /// <returns>The list of entities, in dictionary enumeration order.</returns>
        /// <exception
        public static List<LiteDbConnectorEntity> ToLiteDbConnectorEntityList<T>(this Dictionary<string, T> values, TimeSpan? expiration = null)
        {
            if (values == null)
                return [];

            var connectorEntities = new List<LiteDbConnectorEntity>();
            foreach (var entry in values)
            {
                if (string.IsNullOrEmpty(entry.Key))
                    throw new ArgumentException("Keys cannot be null or empty.", nameof(values));
                if (entry.Value == null)
                    throw new ArgumentException($"Value for key '{entry.Key}' cannot be null.", nameof(values));
                connectorEntities.Add(new LiteDbConnectorEntity(entry.Key, entry.Value, expiration));
            }

            return connectorEntities;
        }

        /// <summary>
        /// Converts a dictionary of key/payload pairs into a list of MongoConnectorEntity.
        /// </summary>
        /// <typeparam name="T">The generic type to store.</typeparam>
        /// <param name="values">The dictionary of key/payload pairs to store. A null dictionary yields an empty list.</param>
        /// <param name="expiration">Optional expiration applied to every entity.</param>
        /// <returns>The list of entities, in dictionary enumeration order.</returns>
        /// <exception cref="ArgumentException">Thrown when a key is null or empty, or a value is null.</exception>
        public static List<MongoConnectorEntity> ToMongoDbConnectorEntityList<T>(this Dictionary<string, T> values, TimeSpan? expiration = null)
        {
            if (values == null)
                return [];

            var connectorEntities = new List<MongoConnectorEntity>();
            foreach (var entry in values)
            {
                if (string.IsNullOrEmpty(entry.Key))
                    throw new ArgumentException("Keys cannot be null or empty.", nameof(values));
                if (entry.Value == null)
                    throw new ArgumentException($"Value for key '{entry.Key}' cannot be null.", nameof(values));
                connectorEntities.Add(new MongoConnectorEntity(entry.Key, entry.Value, expiration));
            }

            return connectorEntities;
        }
    }
}
