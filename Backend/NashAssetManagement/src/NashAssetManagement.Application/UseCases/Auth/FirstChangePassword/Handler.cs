using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Auth.FirstChangePassword
{
    public class Handler(
        UserManager<User> userManager,
        ICurrentUser currentUser,
        IValidator<Request> validator,
        IUnitOfWork uow)
        : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(
            Request orgReq,
            CancellationToken cancellationToken)
        {
            // Pre-cleaning
            var request = orgReq with
            {
                CurrentPassword = orgReq.CurrentPassword.Trim(),
                NewPassword = orgReq.NewPassword.Trim()
            };

            // Validation
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Only authenticated user can change its own password
            if (currentUser == null || !currentUser.IsAuthenticated || currentUser.UserId == null)
            {
                return Errors.Forbidden;
            }

            // Check if user not found or disabled
            var user = await userManager.FindByIdAsync(currentUser.UserId.ToString() ?? string.Empty);
            if (user is null || user.IsDeleted)
            {
                return Errors.UserNotFound;
            }

            // Check if first login or not 
            if (!user.IsFirstLogin)
            {
                return Errors.NotFirstLogin;
            }

            try
            {
                // Change password 
                var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    var errorDescription = string.Join(" ", result.Errors.Select(e => e.Description));
                    return Error.Validation(
                        code: "FirstChangePassword.Failed",
                        description: errorDescription);
                }

                // Mark as no longer first login
                user.IsFirstLogin = false;

                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return Errors.ChangePasswordFailed;
                }

                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                return Errors.PersistenceFailed;
            }

            return new Response();
        }
    }
}
