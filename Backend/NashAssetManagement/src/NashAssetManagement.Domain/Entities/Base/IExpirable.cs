namespace NashAssetManagement.Domain.Entities.Base
{
    public interface IExpirable
    {
        DateTime CreatedAtUtc { get; set; }
        DateTime ExpiresAtUtc { get; set; }
    }
}