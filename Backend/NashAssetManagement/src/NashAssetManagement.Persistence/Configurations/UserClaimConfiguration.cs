using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        const string TableName = "UserClaims";

        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.ToTable(TableName, DbSchema.Auth);
        }
    }
}
