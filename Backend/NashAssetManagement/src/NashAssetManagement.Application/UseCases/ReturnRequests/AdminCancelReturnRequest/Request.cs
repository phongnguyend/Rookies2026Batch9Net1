using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.AdminCancelReturnRequest
{
    public record Request(
        string? ReturnRequestId)
        : IRequest<ErrorOr<Updated>>;
}
