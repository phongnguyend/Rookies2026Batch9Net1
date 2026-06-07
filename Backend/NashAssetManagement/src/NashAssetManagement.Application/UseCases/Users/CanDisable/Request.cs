using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Users.CanDisable
{
    public sealed record Request(string UserId) : IRequest<ErrorOr<Response>>;
}