using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NashAssetManagement.Domain.Entities.Identity
{
    public sealed class Role : IdentityRole<Guid>
    {
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
