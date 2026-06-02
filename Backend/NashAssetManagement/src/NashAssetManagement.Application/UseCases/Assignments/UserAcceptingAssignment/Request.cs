using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assignments.UserAcceptingAssignment
{
    public record Request(
        string? AssignmentId)
        : IRequest<ErrorOr<Updated>>;
}
