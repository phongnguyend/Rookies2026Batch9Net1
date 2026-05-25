using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using Microsoft.EntityFrameworkCore;

namespace NashAssetManagement.Application.UseCases.Users.ViewDetail
{
    internal class Handler(UserManager<User> userManager, ICurrentUser currentUser)
    : IRequestHandler<Query, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(Query query, CancellationToken cancellationToken)
        {
            // Get current admin's location
            // var currentAdminUserId = currentUser.UserId;
            // if (currentAdminUserId == null)
            //     return Errors.Unauthorized();
            // var currentAdmin = await userManager.FindByIdAsync(currentAdminUserId.ToString());
            // if (currentAdmin == null)
            //     return Errors.UserNotFound();

            // Get user by id
            var user = await userManager.Users
                .Include(u => u.Location)
                .FirstOrDefaultAsync(u => u.Id.Equals(query.Id), cancellationToken);
            if (user == null)
                return Errors.UserWithIdNotFound(query.Id);

            // Check if user and current admin have same location
            // if (user.LocationId != currentAdmin.LocationId)
            //     return Errors.Unauthorized();

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