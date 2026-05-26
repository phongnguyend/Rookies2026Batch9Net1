using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        const string TableName = "RoleClaims";

        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.ToTable(TableName, DbSchema.Auth);
        }
    }
}
