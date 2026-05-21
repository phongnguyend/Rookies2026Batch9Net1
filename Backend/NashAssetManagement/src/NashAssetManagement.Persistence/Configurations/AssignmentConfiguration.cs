using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        const string TableName = "assignments";

        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.Property(x => x.AssignedDateAtUtc)
            .HasColumnType("date");

            builder.Property(x => x.Note)
                .HasMaxLength(500);

            builder.HasOne(x => x.Asset)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.AssignedByUser)
                .WithMany(x => x.AssignedAssignments)
                .HasForeignKey(x => x.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AssignedToUser)
                .WithMany(x => x.ReceivedAssignments)
                .HasForeignKey(x => x.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
