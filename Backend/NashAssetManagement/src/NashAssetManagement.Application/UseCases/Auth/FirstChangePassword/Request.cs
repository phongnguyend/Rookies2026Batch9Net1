using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Auth.FirstChangePassword
{
    public record Request(string NewPassword) : IRequest<ErrorOr<Response>>;
}
