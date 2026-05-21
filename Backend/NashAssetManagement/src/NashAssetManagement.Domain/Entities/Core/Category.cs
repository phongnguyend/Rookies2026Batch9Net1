using NashAssetManagement.Domain.Entities.Base;

namespace NashAssetManagement.Domain.Entities.Core
{
    public sealed class Category : BaseEntity<Guid>, ISoftDeletable
    {
        public string CategoryName { get; set; } = default!;

        public string Prefix { get; set; } = default!;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAtUtc { get; set; }

        public ICollection<Asset> Assets { get; set; } = [];
    }
}
