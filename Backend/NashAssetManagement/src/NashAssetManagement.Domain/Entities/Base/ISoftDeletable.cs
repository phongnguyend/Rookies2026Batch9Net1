namespace NashAssetManagement.Domain.Entities.Base
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAtUtc { get; set; }
    }
}
