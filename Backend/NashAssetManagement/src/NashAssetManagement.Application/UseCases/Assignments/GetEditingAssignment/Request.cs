using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assignments.GetEditingAssignment
{
    public record Request(
        string? AssignmentId)
        : IRequest<ErrorOr<Response>>;
}
