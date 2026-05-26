namespace NashAssetManagement.Application.UseCases.ReturnRequests.ViewList
{
    public record Response(
        Guid Id,
        string AssetCode,
        string AssetName,
        string RequestedBy,
        DateTime AssignedDate,
        string? AcceptedBy,
        DateTime? ReturnedDate,
        string State
    );
}