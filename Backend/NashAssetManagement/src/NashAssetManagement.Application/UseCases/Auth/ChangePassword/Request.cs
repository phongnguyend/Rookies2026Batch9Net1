using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Auth.ChangePassword
{
    public record Request(string OldPassword, string NewPassword) : IRequest<ErrorOr<Response>>;
}