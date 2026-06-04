using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assignments.AdminEditAssignment
{
    public record Request(
        string? AssignmentId,
        AssignmentEditPayload? Payload)
        : IRequest<ErrorOr<Response>>;

    public record AssignmentEditPayload(
        string? UserId,
        string? AssetId,
        DateTime AssignedDate,
        string? Note);
}
