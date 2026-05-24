namespace NashAssetManagement.Application.UseCases.Assignments.GetById
{
    public record Response(
        Guid Id,
        string AssetCode,
        string AssetName,
        string Specification,
        string AssignedTo,
        string AssignedBy,
        string AssignedDate,
        string State,
        string Note);
}
