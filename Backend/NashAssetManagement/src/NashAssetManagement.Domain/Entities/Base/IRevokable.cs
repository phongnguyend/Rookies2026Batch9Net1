namespace NashAssetManagement.Domain.Entities.Base
{
    public interface IRevokable
    {
        bool IsRevoked { get; set; }
        DateTime? RevokedAtUtc { get; set; }
    }
}