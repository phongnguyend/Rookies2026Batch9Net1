using NashAssetManagement.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Domain.Entities.Core
{
    public sealed class Category : BaseEntity<Guid>, ISoftDeletable
    {
        public string CategoryName { get; set; } = default!;

        public string Prefix { get; set; } = default!;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAtUtc { get; set; }

        public ICollection<Asset> Assets { get; set; } = [];
    }
}
