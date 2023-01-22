// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return (T)Convert.ChangeType(connectorEntity.Payload, typeof(T));
        }

        /// <summary>
        /// Convert the LiteDbConnectorEntity object to original object.
        /// </summary>
        /// <typeparam name="T">The original object stored.</typeparam>
        /// <param name="liteDbConnectorEntity">The LiteDbConnectorEntity object to convert.</param>
        /// <returns></returns>
        public static T ToPayloadObject<T>(this LiteDbConnectorEntity liteDbConnectorEntity)
        {
            return (T)Convert.ChangeType(liteDbConnectorEntity.Payload, typeof(T));
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
                    result.Add((T)Convert.ChangeType(entity.Payload, typeof(T)));
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
                    result.Add((T)Convert.ChangeType(entity.Payload, typeof(T)));
            }
            return result;
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
                return new List<ConnectorEntity>();

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
                return new List<LiteDbConnectorEntity>();

            var connectorEntities = new List<LiteDbConnectorEntity>();
            foreach (var entry in values)
            {
                if (string.IsNullOrEmpty(entry.Key) || entry.Value == null)
                    continue;
                connectorEntities.Add(new LiteDbConnectorEntity(entry.Key, entry.Value, expiration));
            }

            return connectorEntities;
        }
    }
}
