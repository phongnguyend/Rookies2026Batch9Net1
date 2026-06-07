using Ardalis.Specification;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Users.CanDisable
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
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;
            var currentUserId = user.UserId ?? Guid.Empty;
            if (currentUserId == Guid.Empty) return Errors.UnidentifiedUser;

            try
            {
                var currentUser = await userManager.FindByIdAsync(currentUserId.ToString());
                if (currentUser is null) return Errors.UnidentifiedUser;

                // only allow admin to read
                var isAdmin = await userManager.IsInRoleAsync(currentUser, ApplicationRole.Admin);
                if (!isAdmin) return Errors.OnlyAdminCanDisableUser;

                var targetUser = await userManager.FindByIdAsync(request.UserId.ToString());
                if (targetUser is null) return Errors.UserNotFound;

                var hasValidAssignments = await assignmentRepository.AnyAsync(
                    new UserCanDisableSpecification(targetUser.Id),
                    cancellationToken);

                return new Response(TargetUserId: targetUser.Id, CanDisable: !hasValidAssignments);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to check disable status for UserId {UserId}",
                    request.UserId);

                return Errors.CheckUserDisableFailed;
            }
        }
    }
}