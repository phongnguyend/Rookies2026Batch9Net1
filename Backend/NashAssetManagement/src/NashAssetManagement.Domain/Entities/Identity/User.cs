using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Domain.Entities.Base;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NashAssetManagement.Domain.Entities.Identity
{
    public sealed class User : IdentityUser<Guid>, ITrackable, ISoftDeletable
    {
        public string StaffCode { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime? DateOfBirth { get; set; }
        public DateTime JoinedDateAtUtc {  get; set; }

        public Gender Gender { get; set; }

        public UserType UserType { get; set; }

        public bool IsFirstLogin { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public int LocationId { get; set; }

        public Location Location { get; set; } = default!;

        public ICollection<UserRole> UserRoles { get; set; } = [];

        public ICollection<Assignment> AssignedAssignments { get; set; } = [];

        public ICollection<Assignment> ReceivedAssignments { get; set; } = [];

        public ICollection<ReturnRequest> RequestedReturnRequests { get; set; } = [];
        public ICollection<ReturnRequest> AcceptedReturnRequests { get; set; } = [];
    }
}