using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.DeleteAssignment
{
    internal class Handler(
        IRepository<Assignment, Guid> repo,
        IUnitOfWork uow,
        ICurrentUser currentUser,
        IDateTimeProvider dateTimeProvider,
        IValidator<Request> validator,
        ILogger<Handler> logger
    )
        : IRequestHandler<Request, ErrorOr<Deleted>>
    {
        public async Task<ErrorOr<Deleted>> Handle(Request request, CancellationToken cancellationToken)
        {
            // Validation
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
            // User and user's auth
            if (!currentUser.IsAuthenticated)
                return Errors.UnauthorizedUser;
            var userId = currentUser.UserId;
            if (userId == null)
                return Errors.UnidentifiedUser;
            var assignmentId = Guid.Parse(request.AssignmentId!);
            var assignment = await repo.FirstOrDefaultAsync(new Spec(assignmentId), cancellationToken);
            // Assignment
            if (assignment == null)
                return Errors.AssignmentNotFoundWithId(assignmentId.ToString());
            if (assignment.State != Domain.Enums.AssignmentState.WaitingForAcceptance)
                return Errors.InvalidAssignmentState;
            // Asset
            if (assignment.Asset == null)
                return Errors.AssetOfAssignmentNotFound(assignmentId.ToString());
            if (assignment.Asset.State != Domain.Enums.AssetState.Assigned)
                return Errors.InvalidAssignmentAssetState;
            if (assignment.Asset.LocationId.ToString() != currentUser.LocationId)
                return Errors.InvalidAssignmentAssetLocation;

            try
            {
                assignment.Delete(dateTimeProvider.UtcNow);
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Deleted;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete assignment with Id '{AssignmentId}', request initialized by user '{UserId}'", assignmentId, userId);
                return Errors.UnexpectedErrorOccurred;
            }
        }
    }
}