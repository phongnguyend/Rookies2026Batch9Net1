using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using Microsoft.EntityFrameworkCore;

namespace NashAssetManagement.Application.UseCases.Users.ViewDetail
{
    internal class Handler(UserManager<User> userManager, ICurrentUser currentUser)
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

            // Get user by id
            var user = await userManager.Users
                .Include(u => u.Location)
                .FirstOrDefaultAsync(u => u.Id.Equals(request.Id), cancellationToken);
            if (user == null)
                return Errors.UserWithIdNotFound(request.Id);

            // Check if user and current admin have same location
            if (!user.LocationId.ToString().Equals(currentUser.LocationId))
                return Errors.UserHasDifferentLocation(request.Id);

            return new Response(
                user.Id,
                user.StaffCode,
                user.FirstName + " " + user.LastName,
                user.UserName ?? "",
                user.JoinedAtUtc.ToString("yyyy-MM-dd"),
                user.UserType.ToString(),
                user.DateOfBirth == null ? "" : user.DateOfBirth.Value.ToString("yyyy-MM-dd"),
                user.Gender.ToString(),
                user.Location != null ? user.Location.Name : ""
            );
        }
    }
}
