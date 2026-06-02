using ErrorOr;
using MediatR;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Users.EditUser
{
    public record Request(
        string? UserId,
        DateTime DateOfBirth,
        Gender Gender,
        DateTime JoinedDate,
        UserType Type
    )
    : IRequest<ErrorOr<Response>>;
}