using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Auth.Login
{
    public record Request(string Username, string Password) : IRequest<ErrorOr<Response>> { };
}