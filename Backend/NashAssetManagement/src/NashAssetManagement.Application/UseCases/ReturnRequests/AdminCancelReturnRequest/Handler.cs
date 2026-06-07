using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.AdminCancelReturnRequest
{
    internal class Handler(
        IRepository<ReturnRequest, Guid> repo,
        IUnitOfWork uow,
        ICurrentUser user,
        ILogger<Handler> logger,
        IDateTimeProvider dateTimeProvider,
        IValidator<Request> validator)
        : IRequestHandler<Request, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(Request request, CancellationToken cancellationToken)
        {
            var validationResults = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResults.IsValid) throw new ValidationException(validationResults.Errors);


            // Validate user permission and location
            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;
            if (!user.Roles.Contains(ApplicationRole.Admin)) return Errors.NotAdminUser;
            if (!Guid.TryParse(user.LocationId, out Guid userLocationId)) return Errors.LocationNotFound;

            // Validate return request
            var returnRequest = await repo.FirstOrDefaultAsync(new Spec(Guid.Parse(request.ReturnRequestId!)), cancellationToken);
            if (returnRequest == null) return Errors.RequestNotFound(request.ReturnRequestId!);
            if (returnRequest.Assignment == null) return Errors.AssignmentNotFound;
            if (!returnRequest.CanCancel()) return Errors.InvalidRequestState;

            try
            {
                var timeStamp = dateTimeProvider.UtcNow;
                returnRequest.State = Domain.Enums.ReturnRequestState.Cancelled;
                returnRequest.UpdatedAtUtc = timeStamp;
                returnRequest.Assignment.IsReturning = false;
                returnRequest.Assignment.UpdatedAtUtc = timeStamp;
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Updated;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred when trying to cancel return request with ID '{ReturnRequestId}.'", request.ReturnRequestId);
                return Errors.UnexpectedErrorOccurred;
            }
        }
    }
}
