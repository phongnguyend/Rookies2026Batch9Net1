using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignmentDetail
{
    public record Request(
        string? AssignmentId)
        : IRequest<ErrorOr<Response>>;
}
