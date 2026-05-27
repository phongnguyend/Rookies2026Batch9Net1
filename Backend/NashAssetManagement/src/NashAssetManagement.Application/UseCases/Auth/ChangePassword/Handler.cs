using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Auth.ChangePassword
{
    public class Handler(
        ILogger<Handler> logger,
        UserManager<User> userManager,
        ICurrentUser currentUser,
        IValidator<Request> validator) : IRequestHandler<Request, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(Request orgReq, CancellationToken cancellationToken)
        {
            // cleaning request
            var request = orgReq with
            {
                OldPassword = orgReq.OldPassword.Trim(),
                NewPassword = orgReq.NewPassword.Trim()
            };

            // validation
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Find user by id
            if (currentUser is null || currentUser.UserId is null)
            {
                return Errors.UserNotFound;
            }

            var userId = currentUser.UserId.ToString();

            var user = await userManager.FindByIdAsync(userId!);

            if (user is null)
            {
                return Errors.UserNotFound;
            }

            // Check old password
            var isOldPasswordValid = await userManager.CheckPasswordAsync(user, request.OldPassword);

            if (!isOldPasswordValid)
            {
                return Errors.IncorrectOldPassword;
            }

            // Change Password
            try
            {
                var changePasswordResult = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    var errorCodes = string.Join(", ", changePasswordResult.Errors.Select(e => e.Code));

                    logger.LogError(
                        "Password change failed for user {UserId}. Errors: {Errors}", 
                        userId, 
                        errorCodes);

                    return Errors.ChangePasswordFailed;
                }

                logger.LogInformation(
                    "User {UserId} changed password successfully",
                    userId);

                return Result.Updated;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while changing password");
                return Errors.UnexpectedError;
            }
        }
    }
}