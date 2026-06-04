using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.AdminCompleteReturnRequest
{
    internal class Handler(
        IRepository<ReturnRequest, Guid> repository,
        IUnitOfWork uow,
        ILogger<Handler> logger,
        ICurrentUser currentUser,
        IValidator<Request> validator,
        IDateTimeProvider dateTimeProvider)
        : IRequestHandler<Request, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(Request orgRequest, CancellationToken cancellationToken)
        {
            // pre-cleanning
            var returnRequestId = Guid.Parse(orgRequest.ReturnRequestId!);

            // validation
            var validationResult = await validator.ValidateAsync(orgRequest, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // check current user
            if(currentUser is null || currentUser.UserId is null)
            {
                return Errors.UserNotFound;
            }

            // check return request
            var returnRequest = await repository
                .FirstOrDefaultAsync(new Specification(returnRequestId), cancellationToken);

            if (returnRequest is null)
            {
                return Errors.ReturnRequestNotFound;
            }

            // check return request state [WaitingForReturning]
            switch (returnRequest.State)
            {
                case ReturnRequestState.WaitingForReturning:
                    break;

                case ReturnRequestState.Completed:
                    return Errors.RequestAlreadyCompleted;

                case ReturnRequestState.Cancelled:
                    return Errors.RequestCancelled;

                default:
                    return Errors.InvalidRequestState;
            }

            // check assignment
            if (returnRequest.Assignment is null)
            {
                return Errors.AssignmentNotFound;
            }

            // check assignment state [Accepted]
            if (returnRequest.Assignment.State != AssignmentState.Accepted)
            {
                return Errors.InvalidAssignmentState;
            }

            // check asset
            if (returnRequest.Assignment.Asset is null)
            {
                return Errors.AssetNotFound;
            }

            // Complete Return Request
            try
            {
                // update return request [Completed]
                returnRequest.State = ReturnRequestState.Completed;
                returnRequest.ReturnedAtUtc = dateTimeProvider.UtcNow.Date;

                // update assignment [Returned]
                returnRequest.Assignment.State = AssignmentState.Returned;
                returnRequest.Assignment.IsReturning = false;

                // update asset state [Available]
                returnRequest.Assignment.Asset.State = AssetState.Available;

                await uow.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Admin {UserId} completed returning request {ReturnRequestId} successfully",
                    currentUser.UserId,
                    returnRequestId);

                return Result.Updated;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Unexpected error occurred while admin completed returning request {ReturnRequestId}",
                    returnRequestId);

                return Errors.UnexpectedError;
            }
        }
    }
}