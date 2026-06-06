using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assignments.DeleteAssignment
{
    public record Request(
        string? AssignmentId
    )
    : IRequest<ErrorOr<Deleted>>;
}