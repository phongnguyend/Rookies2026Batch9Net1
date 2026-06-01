using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assignments.UserDecliningAssignment
{
    public record Request(
        string? AssignmentId)
        : IRequest<ErrorOr<Updated>>;
}
