namespace NashAssetManagement.Application.UseCases.Assignments.AdminEditAssignment
{
    public record Response(
        Guid Id,
        string AssetCode,
        string AssetName,
        string AssignedTo,
        string AssignedBy,
        string AssignedDate,
        string State);
}
