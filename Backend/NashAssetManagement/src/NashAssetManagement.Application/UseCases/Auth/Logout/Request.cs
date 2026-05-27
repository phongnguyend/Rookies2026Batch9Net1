using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Auth.Logout
{
    public sealed record Request() : IRequest<ErrorOr<Deleted>>;
}