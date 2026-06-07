namespace NashAssetManagement.Application.UseCases.Assignments.GetEditingAssignment
{
    public record Response(
        Guid Id,
        Assignee User,
        AssignmentAsset Asset,
        DateTime AssignedDate,
        string? Note);

    public record Assignee(
        Guid Id,
        string StaffCode,
        string FullName,
        string Type);

    public record AssignmentAsset(
        Guid Id,
        string AssetCode,
        string AssetName,
        string Category);
}
