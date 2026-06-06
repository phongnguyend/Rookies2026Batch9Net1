using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Users.Disable
{
    public sealed class Handler(
        UserManager<User> userManager,
        IRepository<Assignment, Guid> assignmentRepository,
        ICurrentUser user,
        IValidator<Request> validator,
        ILogger<Handler> logger)
        : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            // Validation
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Implement Logic
            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;

            var currentUserId = user.UserId ?? Guid.Empty;
            if (currentUserId == Guid.Empty) return Errors.UnidentifiedUser;

            try
            {
                var targetUser = await userManager.FindByIdAsync(request.TargetUserId);
                if (targetUser is null) return Errors.UserNotFound;

                // Cannot re-disable again, avoid accessing and modifying the database many times
                // DoS may occurs 
                if (targetUser.IsDeleted) return Errors.UserAlreadyDisabled;

                // Cannot disable yourself
                if (targetUser.Id == currentUserId) return Errors.CannotDisableYourself;

                // Cannot disable target having valid assignments
                var hasValidAssignments = await assignmentRepository.AnyAsync(new UserValidAssignmentsSpecification(targetUser.Id), cancellationToken);
                if (hasValidAssignments) return Errors.UserHasValidAssignments;

                targetUser.IsDeleted = true;
                var updateResult = await userManager.UpdateAsync(targetUser);
                if (!updateResult.Succeeded) return Errors.DisableUserFailed;

                return new Response(targetUser.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to disable user {UserId}", request.TargetUserId);
                return Errors.DisableUserFailed;
            }
        }
    }
}