using NashAssetManagement.Domain.Entities.Base;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Domain.Entities.Core
{
    public sealed class Assignment : TrackableEntity<Guid>
    {
        public DateTime AssignedDateAtUtc { get; set; }

        public string? Note { get; set; }

        public AssignmentState State { get; set; }

        public bool IsReturning { get; set; } = false;

        public int AssetId { get; set; }

        public Asset Asset { get; set; } = default!;

        public int AssignedByUserId { get; set; }

        public User AssignedByUser { get; set; } = default!;

        public int AssignedToUserId { get; set; }

        public User AssignedToUser { get; set; } = default!;

        public ICollection<ReturnRequest> ReturnRequests { get; set; } = [];
    }
}
