using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.UserCreateReturnRequest
{
    internal class Handler(
        IRepository<Assignment, Guid> repo,
        IUnitOfWork uow,
        ILogger<Handler> logger,
        ICurrentUser user,
        IValidator<Request> validator,
        IDateTimeProvider dateTimeProvider)
        : IRequestHandler<Request, ErrorOr<Created>>
    {
        public async Task<ErrorOr<Created>> Handle(Request request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
            var assignmentId = Guid.Parse(request.AssignmentId!);

            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;
            var userId = user.UserId;
            if (userId == null) return Errors.UnidentifiedUser;

            var assignment = await repo.FirstOrDefaultAsync(new Spec(assignmentId), cancellationToken);
            if (assignment == null) return Errors.AssignmentNotFoundWithId(request.AssignmentId!);
            if (assignment.State != Domain.Enums.AssignmentState.Accepted) return Errors.InvalidAssignmentState;

            if (assignment.AssignedToUserId != userId) return Errors.AssignmentNotAssignedToUser;
            bool canCreateRequest =
                !assignment.IsReturning &&
                !assignment.ReturnRequests.Any(x => x.State == Domain.Enums.ReturnRequestState.WaitingForReturning);
            if (!canCreateRequest) return Errors.AssignmentHasWaitingReturnRequest;

            try
            {
                var dateTimeNow = dateTimeProvider.UtcNow;
                var createRequest = ReturnRequest.Create(assignmentId, userId.Value, dateTimeNow);
                assignment.ReturnRequests.Add(createRequest);
                assignment.IsReturning = true;
                assignment.UpdatedAtUtc = dateTimeNow;
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Created;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create return request for assignment with Id '{AssignmentId}', request initialized by user '{UserId}'", assignmentId, userId);
                return Errors.UnexpectedErrorOccurred;
            }
        }
    }
}
