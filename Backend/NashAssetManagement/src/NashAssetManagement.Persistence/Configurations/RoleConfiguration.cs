using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        const string TableName = "Roles";
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(TableName, DbSchema.Auth);
        }
    }
}
