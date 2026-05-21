using NashAssetManagement.Domain.Entities.Base;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Domain.Entities.Core
{
    public sealed class Asset : BaseEntity<Guid>, ISoftDeletable
    {
        public string AssetCode { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Specification { get; set; } = default!;

        public DateTime InstalledAtUtc { get; set; }

        public AssetState State { get; set; }

        public Guid LocationId { get; set; }

        public Location? Location { get; set; }

        public Guid CategoryId { get; set; }

        public Category? Category { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = [];
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
    }
}
