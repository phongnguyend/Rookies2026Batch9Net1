using NashAssetManagement.Domain.Entities.Base;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Domain.Entities.Core
{
    public sealed class Location : BaseEntity<Guid>, ISoftDeletable
    {
        public string Name { get; set; } = default!;

        public string Prefix { get; set; } = default!;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAtUtc { get; set; }

        public ICollection<User> Users { get; set; } = [];

        public ICollection<Asset> Assets { get; set; } = [];
    }
}
