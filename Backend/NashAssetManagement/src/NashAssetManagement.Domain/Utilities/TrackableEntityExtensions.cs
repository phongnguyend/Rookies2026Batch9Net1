using NashAssetManagement.Domain.Entities.Base;

namespace NashAssetManagement.Domain.Utilities
{
    public static class TrackableEntityExtensions
    {
        public static void SetUpdatedDateTime(
            this ITrackable trackableEntity,
            DateTime? updatedTimestampUtc = null)
        {
            trackableEntity.UpdatedAtUtc = updatedTimestampUtc ?? DateTime.UtcNow;
        }
    }
}
