using NashAssetManagement.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Domain.Common
{
    public static class TrackableEntityExtensions
    {
        public static void SetUpdatedDateTime<TKey>(
            this TrackableEntity<TKey> trackableEntity,
            DateTime? updatedDateTime = null)
        {
            trackableEntity.UpdatedAtUtc = updatedDateTime ?? DateTime.UtcNow;
        }
    }
}
