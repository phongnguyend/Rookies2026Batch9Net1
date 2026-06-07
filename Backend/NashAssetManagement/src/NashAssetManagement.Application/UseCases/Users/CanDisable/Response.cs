namespace NashAssetManagement.Application.UseCases.Users.CanDisable
{
    public sealed record Response(Guid TargetUserId, bool CanDisable);
}