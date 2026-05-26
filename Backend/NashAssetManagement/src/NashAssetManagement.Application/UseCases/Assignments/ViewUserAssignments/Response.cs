namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignments
{
    public record Response(
        Guid Id,
        string AssetCode,
        string AssetName,
        string Category,
        DateTime AssignedDate,
        string State,
        bool IsReturning);
}
