using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.AdminCompleteReturnRequest
{
    public record Request(string? ReturnRequestId) : IRequest<ErrorOr<Updated>>;
}
