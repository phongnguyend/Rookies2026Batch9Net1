using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Users.Disable
{
    public sealed record Request(string TargetUserId) : IRequest<ErrorOr<Response>>;
}