// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpConnector.Utilities
{
    /// <summary>
    /// Helpers for handling entity expiration consistently across connectors.
    /// </summary>
    public static class ExpirationHelper
    {
        /// <summary>
        /// Returns null if the entity is expired; otherwise the entity itself.
        /// </summary>
        public static T DiscardIfExpired<T>(this T entity) where T : ConnectorEntity =>
            entity == null || entity.IsExpired() ? null : entity;

        /// <summary>
        /// Filters out null and expired entities.
        /// </summary>
        public static IEnumerable<T> WhereNotExpired<T>(this IEnumerable<T> entities) where T : ConnectorEntity =>
            entities?.Where(e => e != null && !e.IsExpired()) ?? [];

        /// <summary>
        /// Returns the TTL to apply to a native expiring store, or null when the entity never expires.
        /// </summary>
        public static TimeSpan? RemainingTimeToLive(this ConnectorEntity entity)
        {
            if (entity?.ExpiresAt == null)
                return entity?.Expiration;
            var remaining = entity.ExpiresAt.Value.ToUniversalTime() - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.FromMilliseconds(1);
        }
    }
}
