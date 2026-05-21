using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        const string TableName = "user_logins";
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable(TableName, DbSchema.Auth);
        }
    }
}
