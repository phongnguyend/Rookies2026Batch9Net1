using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class AssetConfiguration : IEntityTypeConfiguration<Asset>
    {
        const string TableName = "assets";

        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.Property(x => x.AssetCode)
            .HasMaxLength(10);

            builder.HasIndex(x => x.AssetCode)
                .IsUnique();

            builder.Property(x => x.Name)
                .HasMaxLength(100);

            builder.Property(x => x.Specification)
                .HasMaxLength(500);

            builder.Property(x => x.InstalledDateAtUtc)
                .HasColumnType("date");

            builder.HasOne(x => x.Location)
                .WithMany(x => x.Assets)
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Assets)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
