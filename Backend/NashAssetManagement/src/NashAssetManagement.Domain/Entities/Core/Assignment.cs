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

        public void Decline(DateTime? declineTimestamp = null)
        {
            ArgumentNullException.ThrowIfNull(Asset);
            if (Asset.State != AssetState.Assigned)
                throw new InvalidOperationException($"Assignment contains asset with invalid state for declining, asset's state: {Asset.State}");
            State = AssignmentState.Declined;
            UpdatedAtUtc = declineTimestamp;
            // Release asset back for other assignment to use
            Asset.State = AssetState.Available;
        }

        public void Accept(DateTime? acceptTimestamp = null)
        {
            ArgumentNullException.ThrowIfNull(Asset);
            if (Asset.State != AssetState.Assigned)
                throw new InvalidOperationException($"Assignment contains asset with invalid state for accepting, asset's state: {Asset.State}");
            State = AssignmentState.Accepted;
            UpdatedAtUtc = acceptTimestamp;
        }

        // Must be in WaitingForAcceptance state, not deleted, have assignee and asset.
        public bool CanEdit() =>
            State == AssignmentState.WaitingForAcceptance &&
            !IsDeleted &&
            AssignedToUser != null &&
            Asset != null;

        public void Delete(DateTime? deleteTimestamp = null)
        {
            ArgumentNullException.ThrowIfNull(Asset);
            if (Asset.State != AssetState.Assigned)
                throw new InvalidOperationException($"Assignment contains asset with invalid state for accepting, asset's state: {Asset.State}");
            IsDeleted = true;
            DeletedAtUtc = deleteTimestamp;
            // Release asset back for other assignment to use
            Asset.State = AssetState.Available;
        }
    }
}
