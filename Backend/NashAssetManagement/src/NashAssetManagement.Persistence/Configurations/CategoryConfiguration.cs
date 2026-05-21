using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        const string TableName = "categories";

        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.Property(x => x.CategoryName)
                .HasMaxLength(100);

            builder.HasIndex(x => x.CategoryName)
                .IsUnique();

            builder.Property(x => x.Prefix)
                .HasMaxLength(10);

            builder.HasIndex(x => x.Prefix)
                .IsUnique();
        }
    }
}
