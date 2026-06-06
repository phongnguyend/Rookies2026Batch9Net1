using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.AdminCreateReturnRequest
{
    public record Request(
       string? AssignmentId)
       : IRequest<ErrorOr<Created>>;
}
