using System.Globalization;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Auth.ChangePassword
{
    public class Handler(
        ILogger<Handler> logger,
        UserManager<User> userManager,
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

            // Replace hardcode with current user id from JWT (feature/5199_login_into_the_system)
            // Find user by id
            var userId = Guid.Parse("10000000-0000-0000-0000-000000000001");

            var currentUser = await userManager.FindByIdAsync(userId.ToString());

            if (currentUser is null)
            {
                return Errors.UserNotFound;
            }
            // Change Password
            var changePasswordResult = await userManager.ChangePasswordAsync(
                currentUser,
                request.OldPassword,
                request.NewPassword);

            // If incorrect, return Errors.IncorrectOldPassword
            if (!changePasswordResult.Succeeded)
            {
                var isIncorrectPassword = changePasswordResult.Errors.Any(error =>
                    error.Code == "PasswordMismatch");

                if (isIncorrectPassword)
                {
                    return Errors.IncorrectOldPassword;
                }

                return changePasswordResult.Errors
                    .Select(error =>
                        Error.Validation(
                            error.Code,
                            error.Description))
                    .ToList();
            }

            logger.LogInformation(
                "User {UserId} changed password successfully",
                currentUser.Id);

            return new Response("Your password has been changed successfully!");
        }
    }
}