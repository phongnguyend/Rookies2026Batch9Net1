using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Domain.Entities.Auth;
using NashAssetManagement.Domain.Entities.Base;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Jobs;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Domain.Entities.Identity
{
    public sealed class User : IdentityUser<Guid>, ITrackable, ISoftDeletable
    {
        public string StaffCode { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime? DateOfBirth { get; set; }
        public DateTime JoinedAtUtc { get; set; }

        public Gender Gender { get; set; }

        public UserType UserType { get; set; }

        public bool IsFirstLogin { get; set; } = true;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public Guid LocationId { get; set; }

        public Location? Location { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<Assignment> AssignedAssignments { get; set; } = [];
        public ICollection<Assignment> ReceivedAssignments { get; set; } = [];
        public ICollection<ReturnRequest> RequestedReturnRequests { get; set; } = [];
        public ICollection<ReturnRequest> AcceptedReturnRequests { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
        public ExportReportJob? RequestedExportReportJob { get; set; }
    }
}