using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        const string TableName = "roles";
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(TableName, DbSchema.Auth);
        }
    }
}
