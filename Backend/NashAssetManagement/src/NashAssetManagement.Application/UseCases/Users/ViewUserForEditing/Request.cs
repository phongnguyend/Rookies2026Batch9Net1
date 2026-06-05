using MediatR;
using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Users.ViewUserForEditing
{
    public record Request(
        string? UserId
    ) : IRequest<ErrorOr<Response>>;
}