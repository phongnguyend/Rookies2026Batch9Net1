using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Users.ViewDetail
{
    public record Request(
        string? UserId
    ) : IRequest<ErrorOr<Response>>;
}