using NashAssetManagement.Domain.Entities.Base;
using NashAssetManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Domain.Entities.Core
{
    public sealed class Asset : SoftDeletableEntity<Guid>
    {
        public string AssetCode { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Specification { get; set; } = default!;

        public DateTime InstalledDateAtUtc { get; set; }

        public AssetState State { get; set; }

        public int LocationId { get; set; }

        public Location Location { get; set; } = default!;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = default!;

        public ICollection<Assignment> Assignments { get; set; } = [];
    }
}
