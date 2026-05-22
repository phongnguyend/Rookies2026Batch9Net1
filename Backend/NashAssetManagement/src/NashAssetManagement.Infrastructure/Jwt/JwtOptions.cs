using System.ComponentModel.DataAnnotations;

namespace NashAssetManagement.Infrastructure.Jwt
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        [Required, MinLength(1)]
        public string Issuer { get; init; } = default!;
        [Required, MinLength(1)]
        public string Audience { get; init; } = default!;
        [Required, MinLength(1)]
        public string SecretKey { get; init; } = default!;
        [Range(1, long.MaxValue)]
        public long AccessTokenExpiryInMilliseconds { get; init; }
        [Range(1, long.MaxValue)]
        public long RefreshTokenExpiryInMilliseconds { get; init; }
    }
}
