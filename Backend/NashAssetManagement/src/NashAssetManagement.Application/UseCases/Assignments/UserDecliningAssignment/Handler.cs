using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assignments.UserDecliningAssignment
{
    internal class Handler(
        IRepository<Assignment, Guid> repo,
        IUnitOfWork uow,
        ICurrentUser user,
        IValidator<Request> validator,
        IDateTimeProvider dateTimeProvider,
        ILogger<Handler> logger)
        : IRequestHandler<Request, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            // Validation
            // Request
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
            // User and user's auth state
            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;
            var userId = user.UserId;
            if (userId == null) return Errors.UnidentifiedUser;

            var assignmentId = Guid.Parse(request.AssignmentId!);
            var assignment = await repo.FirstOrDefaultAsync(new Spec(assignmentId), cancellationToken);
            // Assignment
            if (assignment == null) return Errors.AssignmentNotFoundWithId(request.AssignmentId!);
            if (assignment.State != AssignmentState.WaitingForAcceptance) return Errors.InvalidAssignmentState;
            if (assignment.AssignedToUserId != userId) return Errors.AssignmentNotAssignedToUser;
            // Assignment's asset
            if (assignment.Asset == null) return Errors.AssetOfAssignmentNotFound(request.AssignmentId!);
            if (assignment.Asset.State != AssetState.Assigned) return Errors.InvalidAssignmentAssetState;

            try
            {
                assignment.Decline(dateTimeProvider.UtcNow);
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Updated;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to decline assignment with Id '{AssignmentId}', request initialized by user '{UserId}'", assignmentId, userId);
                return Errors.UnexpectedErrorOccurred;
            }
        }
    }
}
