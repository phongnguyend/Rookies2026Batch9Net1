using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Auth.Profile
{
    public record Request() : IRequest<ErrorOr<Response>>;
}
