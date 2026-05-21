using Microsoft.AspNetCore.Identity;

namespace NashAssetManagement.Domain.Entities.Identity
{
    public sealed class UserRole : IdentityUserRole<Guid>
    {
        public User User { get; set; } = default!;
        public Role Role { get; set; } = default!;
    }
}
