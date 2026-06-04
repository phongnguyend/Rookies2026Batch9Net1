using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.Realtime;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Auth;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Users.EditUser
{
    internal class Handler(
        UserManager<User> userManager,
        ICurrentUser currentUser,
        IValidator<Request> validator,
        IUnitOfWork unitOfWork,
        IRepository<RefreshToken, Guid> refreshTokenRepository,
        IUserSessionNotifier userSessionNotifier
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

            // Validate request
            var validationResults = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResults.IsValid)
                throw new ValidationException(validationResults.Errors);

            // Get user by id
            var user = await userManager.FindByIdAsync(request.UserId!);
            if (user == null)
                return Errors.UserWithIdNotFound(request.UserId!);

            // Check if user and current admin have same location
            if (!user.LocationId.ToString().Equals(currentUser.LocationId))
                return Errors.UserHasDifferentLocation(request.UserId!);

            // Check if admin edit their own information
            if (request.Type != user.UserType &&
                user.Id.ToString().Equals(currentUser.UserId.ToString()))
                return Errors.AdminNotAllowedToEditOwnType();

            var userTypeChanged = user.UserType != request.Type;

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // Update user role
                var updateRoleResult = await UpdateUserRole(user, request.Type);

                if (updateRoleResult.IsError)
                    return updateRoleResult.Errors;

                // Update user
                user.DateOfBirth = request.DateOfBirth;
                user.Gender = request.Gender;
                user.JoinedAtUtc = request.JoinedDate;
                user.UserType = request.Type;
                user.UpdatedAtUtc = DateTime.UtcNow;

                if (userTypeChanged)
                {
                    var activeRefreshTokens = await refreshTokenRepository
                        .GetQueryableSet()
                        .Where(x => x.UserId == user.Id && !x.IsRevoked && x.ExpiresAtUtc > DateTime.UtcNow)
                        .ToListAsync(cancellationToken);

                    foreach (var token in activeRefreshTokens)
                    {
                        token.IsRevoked = true;
                        token.RevokedAtUtc = DateTime.UtcNow;
                    }
                }

                var updateResult =await userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Errors.FailedToUpdateUser(user.Id.ToString());
                }
                    
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                if (userTypeChanged)
                {
                    await userSessionNotifier.ForceLogoutAsync(
                        user.Id,
                        "Your user type was updated. Please sign in again.",
                        cancellationToken);
                }

                return new Response(user.Id);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Errors.UnexpectedErrorOccurred();
            }         
        }

        private async Task<ErrorOr<Success>> UpdateUserRole(User user, UserType newType)
        {
            var currentRole = (await userManager.GetRolesAsync(user)).SingleOrDefault();

            ErrorOr<string> newRoleResult = newType switch
            {
                UserType.Admin => ApplicationRole.Admin,
                UserType.Staff => ApplicationRole.Staff,
                _ => Errors.InvalidUserType(newType.ToString())
            };

            if (newRoleResult.IsError)
                return newRoleResult.Errors;

            var newRole = newRoleResult.Value;

            if (currentRole == newRole)
                return Result.Success;

            if (currentRole is not null)
            {
                var removeResult = await userManager.RemoveFromRoleAsync(user, currentRole);

                if (!removeResult.Succeeded)
                    return Errors.FailedToUpdateUserRole(user.Id.ToString(), newRole);
            }

            var addResult = await userManager.AddToRoleAsync(user, newRole);

            if (!addResult.Succeeded)
                return Errors.FailedToUpdateUserRole(user.Id.ToString(), newRole);

            await userManager.UpdateSecurityStampAsync(user);

            return Result.Success;
        }
    };
}
