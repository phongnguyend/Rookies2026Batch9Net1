using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Persistence.Configurations
{
    public sealed class ExportReportJobConfiguration : IEntityTypeConfiguration<ExportReportJob>
    {
        const string TableName = "ExportReportJobs";

        public void Configure(EntityTypeBuilder<ExportReportJob> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequestedByAdminId)
                .IsRequired();

            builder.Property(x => x.FilePath)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.CompletedAtUtc)
                .IsRequired(false);

            builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(50); // avoid create max length NVARCHAR(MAX)

            builder.HasIndex(x => x.RequestedByAdminId)
                .IsUnique();

            builder.HasOne(x => x.RequestedByAdmin)
                .WithOne(x => x.RequestedExportReportJob)
                .HasForeignKey<ExportReportJob>(x => x.RequestedByAdminId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}