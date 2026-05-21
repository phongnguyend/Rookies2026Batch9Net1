using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        const string TableName = "user_tokens";
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.ToTable(TableName, DbSchema.Auth);
        }
    }
}
