using NashAssetManagement.Domain.Entities.Base;

namespace NashAssetManagement.Domain.Entities.Auth
{
    public class RefreshToken : BaseEntity<Guid>, IRevokable, IExpirable
    {
        public Guid UserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
    }
}