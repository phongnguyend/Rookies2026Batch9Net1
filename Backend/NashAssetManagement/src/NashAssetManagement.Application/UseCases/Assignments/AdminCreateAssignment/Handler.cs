using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assignments.AdminCreateAssignment
{
    internal class Handler(
        IRepository<Asset, Guid> repoAsset,
        IRepository<Assignment, Guid> repoAssignment,
        ICurrentUser currentUser,
        UserManager<User> userManager,
        IValidator<Request> validator,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork uow,
        ILogger<Handler> logger
        ) : IRequestHandler<Request, ErrorOr<Created>>
    {
        public async Task<ErrorOr<Created>> Handle(Request request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            if (!currentUser.IsAuthenticated) return Errors.UnauthorizedUser;
            var userId = currentUser.UserId;
            if (userId == null) return Errors.UnidentifiedUser;

            var admin = await userManager.FindByIdAsync(userId.ToString()!);
            if (admin == null) return Errors.UnidentifiedUser;

            var staffId = Guid.Parse(request.UserId);
            var staff = await userManager.FindByIdAsync(staffId.ToString()!);
            if (staff == null) return Errors.StaffNotFound;
            if (staff.LocationId != admin.LocationId) return Errors.StaffNotInSameLocation;
            if (staff.IsDeleted == true) return Errors.StaffNotFound;

            var assetId = Guid.Parse(request.AssetId!);
            var asset = await repoAsset.FirstOrDefaultAsync(new AssetSpec(assetId), cancellationToken);
            if (asset == null) return Errors.AssetNotFound;
            if (asset.LocationId != staff.LocationId) return Errors.AssetNotInSameLocation;
            if (asset.State != AssetState.Available) return Errors.AssetNotAvailable;

            var assignment = new Assignment
            {
                AssignedByUserId = admin.Id,
                AssignedToUserId = staff.Id,
                AssetId = asset.Id,
                AssignedAtUtc = request.AssignedDate,
                Note = request.Note,
                State = AssignmentState.WaitingForAcceptance,
                IsReturning = false,
                CreatedAtUtc = dateTimeProvider.UtcNow
            };

            //Update asset state to Assigned
            asset.State = AssetState.Assigned;
            asset.UpdatedAtUtc = dateTimeProvider.UtcNow;

            try
            {
                await repoAssignment.AddAsync(assignment, cancellationToken);
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Created;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create return request for assignment with Id '{AssignmentId}', request initialized by user '{UserId}'", assignment.Id, userId);
                return Errors.UnexpectedErrorOccurred;
            }
        }
    }
}
