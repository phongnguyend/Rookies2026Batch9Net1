using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Users.ViewUserDetail
{
    internal class Handler(
        UserManager<User> userManager, 
        ICurrentUser currentUser,
        IValidator<Request> validator
    )
    : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            // Check current user id
            if (currentUser.UserId == null)
                return Errors.Unauthorized();

            // Check current admin's location
            if (currentUser.LocationId == null)            
                return Errors.UserHasNoLocation();

            var validationResults = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResults.IsValid)
                throw new ValidationException(validationResults.Errors);

            // Get user by id
            var user = await userManager.Users
                .Include(u => u.Location)
                .FirstOrDefaultAsync(u => u.Id.ToString().Equals(request.UserId), cancellationToken);
            if (user == null)
                return Errors.UserWithIdNotFound(currentUser.UserId.ToString()!);

            // Check if user and current admin have same location
            if (!user.LocationId.ToString().Equals(currentUser.LocationId))
                return Errors.UserHasDifferentLocation(currentUser.UserId.ToString()!);

            return new Response(
                user.Id,
                user.StaffCode,
                user.FirstName + " " + user.LastName,
                user.UserName ?? "",
                user.JoinedAtUtc,
                user.UserType.ToString(),
                user.DateOfBirth,
                user.Gender.ToString(),
                user.Location != null ? user.Location.Name : ""
            );
        }
    }
}
