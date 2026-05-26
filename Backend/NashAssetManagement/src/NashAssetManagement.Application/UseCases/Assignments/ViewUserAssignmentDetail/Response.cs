namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignmentDetail
{
    public record Response(
        Guid AssignmentId,
        string AssetCode,
        string AssetName,
        string Specification,
        string AssignerName,
        string AssigneeName,
        DateTime AssignedDate,
        string State,
        string? Note);
}
