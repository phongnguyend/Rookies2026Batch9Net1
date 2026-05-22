using NashAssetManagement.Domain.Entities.Base;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Domain.Entities.Core
{
    public sealed class Assignment : BaseEntity<Guid>, ITrackable, ISoftDeletable
    {
        public DateTime AssignedAtUtc { get; set; }

        public string? Note { get; set; }

        public AssignmentState State { get; set; }

        public bool IsReturning { get; set; } = false;

        public Guid AssetId { get; set; }

        public Asset? Asset { get; set; }

        public Guid AssignedByUserId { get; set; }

        public User? AssignedByUser { get; set; }

        public Guid AssignedToUserId { get; set; }

        public User? AssignedToUser { get; set; }

        public ICollection<ReturnRequest> ReturnRequests { get; set; } = [];
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
    }
}
