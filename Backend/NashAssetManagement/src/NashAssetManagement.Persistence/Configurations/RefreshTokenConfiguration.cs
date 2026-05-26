using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Auth;

namespace NashAssetManagement.Persistence.Configurations
{
    internal sealed class RefreshTokenConfiguration
        : IEntityTypeConfiguration<RefreshToken>
    {
        const string TableName = "RefreshTokens";

        public void Configure(
            EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable(TableName, DbSchema.Auth);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.ExpiresAtUtc)
                .IsRequired();

            builder.Property(x => x.IsRevoked)
                .IsRequired();

            builder.Property(x => x.RevokedAtUtc);

            builder.HasIndex(x => x.UserId);
        }
    }
}