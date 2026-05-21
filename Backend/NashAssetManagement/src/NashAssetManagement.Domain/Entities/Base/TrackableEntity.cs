using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Domain.Entities.Base
{
    public abstract class TrackableEntity<TKey> : BaseEntity<TKey>, ITrackable
    {
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public virtual void MarkEntityAsUpdated(DateTime? updateTimestamp = null)
        {
            UpdatedAtUtc = updateTimestamp ?? DateTime.UtcNow;
        }
    }
}
