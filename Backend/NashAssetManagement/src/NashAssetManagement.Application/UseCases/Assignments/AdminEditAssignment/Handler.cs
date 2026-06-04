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

namespace NashAssetManagement.Application.UseCases.Assignments.AdminEditAssignment
{
    internal class Handler(
        IRepository<Assignment, Guid> assignmentRepo,
        IRepository<Asset, Guid> assetRepo,
        UserManager<User> userManager,
        ICurrentUser user,
        ILogger<Handler> logger,
        IUnitOfWork uow,
        IValidator<Request> validator)
        : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            // Validate request
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
            var assignmentId = Guid.Parse(request.AssignmentId!);

            // Validate user permission and location
            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;
            if (!user.Roles.Contains(ApplicationRole.Admin)) return Errors.NotAdminUser;
            if (!Guid.TryParse(user.LocationId, out Guid userLocationId)) return Errors.LocationNotFound;

            // Validate assignment
            var assignment = await assignmentRepo.FirstOrDefaultAsync(new AssignmentSpec(assignmentId), cancellationToken);
            if (assignment == null) return Errors.AssignmentNotFoundWithId(request.AssignmentId!);
            if (!assignment.CanEdit()) return Errors.InvalidAssignmentState;
            if (userLocationId != assignment.Asset!.LocationId || userLocationId != assignment.AssignedToUser!.LocationId)
                return Errors.CannotEditOtherLocationAssignment;

            // Validate new user
            var newUser = await userManager.FindByIdAsync(request.Payload!.UserId!);
            if (newUser == null) return Errors.NewUserNotFound(request.Payload!.UserId!);
            bool differentUsers = assignment.AssignedToUserId != newUser.Id;
            if (differentUsers)
            {
                if (newUser.IsDeleted) return Errors.CannotAssignDisabledUser;
                if (newUser.LocationId != userLocationId) return Errors.CannotAssignUserFromOtherLocation;
            }

            // Validate new asset
            if (!Guid.TryParse(request.Payload!.AssetId, out Guid assetId)) return Errors.NewAssetNotFound(request.Payload!.AssetId!);
            var newAsset = await assetRepo.FirstOrDefaultAsync(new AssetSpec(assetId, userLocationId), cancellationToken);
            if (newAsset == null) return Errors.NewAssetNotFound(request.Payload!.AssetId!);
            bool differentAssets = assignment.AssetId != newAsset.Id;
            if (differentAssets && !newAsset.IsAssignable()) return Errors.InvalidNewAssetState;

            try
            {
                if (differentUsers)
                {
                    assignment.AssignedToUserId = newUser.Id;
                    assignment.AssignedToUser = newUser;
                }
                if (differentAssets)
                {
                    // Release old asset
                    assignment.Asset.State = Domain.Enums.AssetState.Available;
                    
                    assignment.AssetId = newAsset.Id;
                    assignment.Asset = newAsset;

                    // Claim new asset
                    newAsset.State = Domain.Enums.AssetState.Assigned;
                }
                assignment.AssignedAtUtc = request.Payload.AssignedDate;
                assignment.Note = request.Payload.Note;
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred when trying to edit assignment with Id '{AssignmentId}', request initialized by user '{AdminId}'.",
                    request.AssignmentId,
                    user.UserId);
                return Errors.UnexpectedErrorOccurred;
            }
            return new Response(assignment.Id,
                assignment.Asset.AssetCode,
                assignment.Asset.Name,
                assignment.AssignedToUser!.UserName!,
                assignment.AssignedByUser!.UserName!,
                assignment.AssignedAtUtc.ToString("dd/MM/yyyy"),
                assignment.State.ToString());
        }
    }
}
