using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Users.ViewUserForEditing
{
    internal class Handler(
        UserManager<User> userManager,
        ICurrentUser currentUser,
        IValidator<Request> validator
    )
    : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(
            Request request, 
            CancellationToken cancellationToken)
        {
            // Check userId
            if (currentUser.UserId == null)
                return Errors.Unauthorized();

            // Check locationId
            if (currentUser.LocationId == null)
                return Errors.UserHasNoLocation();

            var validationResults = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResults.IsValid)
                throw new ValidationException(validationResults.Errors);

            // Find user by id
            var user = await userManager.FindByIdAsync(request.UserId!);
            if (user == null)
                return Errors.UserWithIdNotFound(request.UserId!);

            // Check current admin with user have same location
            if (!currentUser.LocationId!.Equals(user.LocationId.ToString()))
                return Errors.UserHasDifferentLocation();

            var response = new Response(
                user.Id,
                user.FirstName,
                user.LastName,
                user.DateOfBirth,
                user.Gender.ToString(),
                user.JoinedAtUtc,
                user.UserType.ToString(),
                user.Id.ToString().Equals(currentUser.UserId.ToString()),
                user.ConcurrencyStamp ?? string.Empty
            );

            return response;
        }
    }
    ;
}
