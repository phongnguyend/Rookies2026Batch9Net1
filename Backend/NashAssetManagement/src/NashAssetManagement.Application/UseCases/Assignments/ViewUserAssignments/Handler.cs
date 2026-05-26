using ErrorOr;
using FluentValidation;
using MediatR;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignments
{
    internal class Handler(
        IRepository<Assignment, Guid> repo,
        ICurrentUser user,
        IValidator<Request> validator,
        IDateTimeProvider dateTimeProvider)
        : IRequestHandler<Request, ErrorOr<PagedList<Response>>>
    {
        public async Task<ErrorOr<PagedList<Response>>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            var cleanedRequest = request with
            {
                SortBy = request.SortBy?.Trim(),
                PageNumber = request.PageNumber ?? 1,
                PageSize = request.PageSize ?? 20,
            };
            var validationResults = await validator.ValidateAsync(cleanedRequest, cancellationToken);
            if (!validationResults.IsValid) throw new ValidationException(validationResults.Errors);

            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;
            var userId = user.UserId;
            if (userId == null) return Errors.UnidentifiedUser;

            var filterSpec = new FilterSpec(userId.Value, dateTimeProvider.UtcNow, cleanedRequest);
            var spec = new PagingAndSortingSpec(userId.Value, dateTimeProvider.UtcNow, cleanedRequest);

            var totalItems = await repo.CountAsync(filterSpec, cancellationToken);
            var items = await repo.ListAsync(spec, cancellationToken);
            return PagedList.Create(items, totalItems, cleanedRequest.PageNumber.Value, cleanedRequest.PageSize.Value);
        }
    }
}
