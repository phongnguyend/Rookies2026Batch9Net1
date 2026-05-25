namespace NashAssetManagement.Application.UseCases.Auth.Profile
{
    public record Response(
        Guid Id,
        string UserName,
        Guid LocationId,
        bool IsFirstLogin,
        IReadOnlyList<string> Roles);
}
