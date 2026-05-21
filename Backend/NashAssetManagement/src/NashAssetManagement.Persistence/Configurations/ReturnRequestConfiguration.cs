using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging.Abstractions;
using NashAssetManagement.Domain.Entities.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class ReturnRequestConfiguration : IEntityTypeConfiguration<ReturnRequest>
    {
        const string TableName = "return_requests";

        public void Configure(EntityTypeBuilder<ReturnRequest> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.Property(x => x.ReturnedDate)
            .HasColumnType("date");

            builder.HasOne(x => x.Assignment)
                .WithMany(x => x.ReturnRequests)
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.RequestedByUser)
                .WithMany(x => x.RequestedReturnRequests)
                .HasForeignKey(x => x.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AcceptedByUser)
                .WithMany(x => x.AcceptedReturnRequests)
                .HasForeignKey(x => x.AcceptedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
