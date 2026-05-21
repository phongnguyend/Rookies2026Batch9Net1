using NashAssetManagement.Domain.Entities.Base;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Domain.Entities.Core
{
    public sealed class ReturnRequest : BaseEntity<Guid>, ITrackable
    {
        public DateTime? ReturnedAtUtc { get; set; }

        public ReturnRequestState State { get; set; }

        public Guid AssignmentId { get; set; }

        public Assignment? Assignment { get; set; }

        public Guid RequestedByUserId { get; set; }

        public User? RequestedByUser { get; set; }

        public Guid? AcceptedByUserId { get; set; }

        public User? AcceptedByUser { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
