namespace NashAssetManagement.Application.UseCases.Users.UsersLookup
{
    public record Response(
        Guid Id,
        string StaffCode,
        string FullName,
        string Type);
}
