using NashAssetManagement.Domain.Entities.Base;

namespace NashAssetManagement.Domain.Utilities
{
    public static class SoftDeletableEntityExtensions
    {
        public static void SoftDelete(
            this ISoftDeletable entity,
            DateTime? deleteTimestampUtc = null)
        {
            entity.IsDeleted = true;
            entity.DeletedAtUtc = deleteTimestampUtc ?? DateTime.UtcNow;
        }

        public static void RestoreSoftDeleted(
            this ISoftDeletable entity)
        {
            entity.IsDeleted = false;
            entity.DeletedAtUtc = null;
        }
    }
}
