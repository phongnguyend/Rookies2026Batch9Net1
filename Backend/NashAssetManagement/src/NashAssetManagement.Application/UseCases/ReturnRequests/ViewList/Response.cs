namespace NashAssetManagement.Application.UseCases.ReturnRequests.ViewList
{
    public record Response(
        Guid Id,
        string AssetCode,
        string AssetName,
        string RequestedBy,
        string AssignedDate,
        string? AcceptedBy,
        string? ReturnedDate,
        string State
    );
}