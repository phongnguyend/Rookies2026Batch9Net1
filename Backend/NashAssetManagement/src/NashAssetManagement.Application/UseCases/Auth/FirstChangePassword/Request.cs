using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Auth.FirstChangePassword
{
    public record Request(string CurrentPassword, string NewPassword) : IRequest<ErrorOr<Response>>;
}
