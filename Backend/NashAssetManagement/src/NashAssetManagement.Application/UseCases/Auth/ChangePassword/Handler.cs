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
        IValidator<Request> validator) : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(Request orgReq, CancellationToken cancellationToken)
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
                return validationResult.Errors
                    .ConvertAll(error =>
                        Error.Validation(
                            error.PropertyName,
                            error.ErrorMessage));
            }

            // Find user by id
            if (currentUser.UserId is null)
            {
                return Errors.UserIdNotFound;
            }

            var userId = currentUser.UserId.ToString();

            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return Errors.UserNotFound;
            }

            // Change Password
            var changePasswordResult = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            // If incorrect, return Errors.IncorrectOldPassword
            if (!changePasswordResult.Succeeded)
            {
                var isIncorrectPassword = changePasswordResult.Errors.Any(error =>
                    error.Code == PasswordConstants.PasswordMismatchCode);

                if (isIncorrectPassword)
                {
                    return Errors.IncorrectOldPassword;
                }

                logger.LogWarning("Password change failed for user {UserId}", userId);

                return changePasswordResult.Errors
                    .Select(error =>
                        Error.Validation(
                            error.Code,
                            error.Description))
                    .ToList();
            }

            logger.LogInformation(
                "User {UserId} changed password successfully",
                userId);

            return new Response("Your password has been changed successfully!");
        }
    }
}