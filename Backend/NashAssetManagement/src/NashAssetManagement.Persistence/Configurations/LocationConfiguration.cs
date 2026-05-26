using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        const string TableName = "Locations";

        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.Property(x => x.Name)
                .HasMaxLength(100);

            builder.HasIndex(x => x.Name)
                .IsUnique();

            builder.Property(x => x.Prefix)
                .HasMaxLength(10);

            builder.HasIndex(x => x.Prefix)
                .IsUnique();
        }
    }
}
