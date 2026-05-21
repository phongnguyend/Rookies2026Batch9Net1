using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NashAssetManagement.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Persistence.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        const string TableName = "users";

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(TableName, DbSchema.Auth);

            builder.Property(x => x.PasswordHash)
            .HasMaxLength(255);

            builder.Property(x => x.UserName)
                .HasMaxLength(100);

            builder.Property(x => x.StaffCode)
                .HasMaxLength(10);

            builder.HasIndex(x => x.StaffCode)
                .IsUnique();

            builder.HasIndex(x => x.UserName)
                .IsUnique();

            builder.Property(x => x.FirstName)
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .HasMaxLength(100);

            builder.Property(x => x.DateOfBirth)
                .HasColumnType("date");

            builder.Property(x => x.JoinedDateAtUtc)
                .HasColumnType("date");
        }
    }
}
