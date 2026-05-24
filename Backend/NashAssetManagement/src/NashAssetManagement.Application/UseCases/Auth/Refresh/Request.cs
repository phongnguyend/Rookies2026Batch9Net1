using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Auth.Refresh
{
    public record Request(string RefreshToken) : IRequest<ErrorOr<Response>>;
}
