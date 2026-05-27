using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Users.ViewUserDetail
{
    public record Request(
        string? UserId
    ) : IRequest<ErrorOr<Response>>;
}