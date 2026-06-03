using NashAssetManagement.Application.Abstractions.AppNamingFormat;
using NashAssetManagement.Domain.Constants;
namespace NashAssetManagement.Infrastructure.AppNamingFormat
{
    public sealed class AppNamingFormat : IAppNamingFormat
    {
        public string GetBaseUserName(string firstName, string lastName)
        {
            var firstNamePart = firstName
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Last();

            var lastNameInitials = lastName
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x[0]);

            return $"{firstNamePart}{string.Concat(lastNameInitials)}"
                .ToLowerInvariant();
        }

        public string GetPassword(string username, DateTime dateOfBirth)
        {
            return $"{username}@{dateOfBirth:ddMMyyyy}";
        }

        public string GetStaffCode(int number)
        {
            return $"{CompanyConstants.StaffCode}{number:D4}";
        }

        public string GetEmail(string username)
        {
            return $"{username}@{CompanyConstants.EmailDomain}";
        }

        public string GetUniqueUserName(string baseUsername, IEnumerable<string> existingUsernames)
        {
            var usernames = existingUsernames.ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!usernames.Contains(baseUsername))
            {
                return baseUsername;
            }

            var maxIndex = usernames
                .Where(x => x.StartsWith(baseUsername, StringComparison.OrdinalIgnoreCase))
                .Select(x => x[baseUsername.Length..])
                .Select(suffix => int.TryParse(suffix, out var number) ? number : 0)
                .DefaultIfEmpty(0)
                .Max();

            return $"{baseUsername}{maxIndex + 1}";
        }
    
    }
}