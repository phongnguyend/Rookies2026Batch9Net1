using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assignments.AdminCreateAssignment
{
    public record Request(
        string UserId,
        string AssetId,
        DateTime AssignedDate,
        string? Note)
        : IRequest<ErrorOr<Created>>;
}
