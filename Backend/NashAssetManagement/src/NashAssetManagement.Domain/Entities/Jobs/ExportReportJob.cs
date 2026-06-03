using NashAssetManagement.Domain.Entities.Base;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Domain.Entities.Jobs
{
    public class ExportReportJob : BaseEntity<Guid>, ICompletable
    {
        public Guid RequestedByAdminId { get; set; }

        public ExportReportJobStatus Status { get; set; }

        public string? FilePath { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        public User RequestedByAdmin { get; set; } = null!;
    }
}