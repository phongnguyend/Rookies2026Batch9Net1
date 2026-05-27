using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.UserCreateReturnRequest
{
    public record Request(
        string? AssignmentId)
        : IRequest<ErrorOr<Created>>;
}
