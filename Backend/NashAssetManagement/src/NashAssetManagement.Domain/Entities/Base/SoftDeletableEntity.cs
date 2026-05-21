using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Domain.Entities.Base
{
    public abstract class SoftDeletableEntity<TKey>
        : TrackableEntity<TKey>, ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }

        public virtual void SoftDelete(DateTime? deleteTimestamp = null)
        {
            IsDeleted = true;
            DeletedAtUtc = deleteTimestamp ?? DateTime.UtcNow;
        }

        public virtual void RestoreSoftDelete()
        {
            IsDeleted = false;
            DeletedAtUtc = null;
        }
    }
}
