using NashAssetManagement.Domain.Entities.Base;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Domain.Entities.Core
{
    public sealed class ReturnRequest : TrackableEntity<Guid>
    {
        public DateTime? ReturnedDate {  get; set; }

        public ReturnRequestState State { get; set; }

        public int AssignmentId { get; set; }

        public Assignment Assignment { get; set; } = default!;

        public int RequestedByUserId { get; set; }

        public User RequestedByUser { get; set; } = default!;

        public int? AcceptedByUserId { get; set; }

        public User? AcceptedByUser { get; set; }
    }
}
