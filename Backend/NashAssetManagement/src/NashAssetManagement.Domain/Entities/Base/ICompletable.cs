namespace NashAssetManagement.Domain.Entities.Base
{
    public interface ICompletable
    {
        DateTime CreatedAtUtc { get; set; }
        DateTime? CompletedAtUtc { get; set; }
    }
}