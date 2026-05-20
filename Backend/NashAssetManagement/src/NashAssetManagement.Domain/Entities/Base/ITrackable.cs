namespace NashAssetManagement.Domain.Entities.Base
{
    public interface ITrackable
    {
        DateTime CreatedAtUtc { get; set; }
        DateTime? UpdatedAtUtc { get; set; }
    }
}
