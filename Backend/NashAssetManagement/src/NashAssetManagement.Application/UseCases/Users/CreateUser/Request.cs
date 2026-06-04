using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Users.CreateUser
{
    public record Request (
        string FirstName,
        string LastName,
        DateTime DayOfBirth,
        DateTime JoinedDate,
        Gender Gender,
        UserType UserType
    ) : IRequest<ErrorOr<Response>>;
}