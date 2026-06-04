using ErrorOr;
using FluentValidation;
using MediatR;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets.AssetsLookupForEditAssignment
{
    internal class Handler(
        IRepository<Asset, Guid> repo,
        ICurrentUser user,
        IValidator<Request> validator)
        : IRequestHandler<Request, ErrorOr<PagedList<Response>>>
    {
        public async Task<ErrorOr<PagedList<Response>>> Handle(Request request, CancellationToken cancellationToken)
        {
            var sanitizedRequest = request with
            {
                SearchTerm = request.SearchTerm?.Trim(),
                SortBy = request.SortBy?.Trim(),
                PageNumber = request.PageNumber ?? 1,
                PageSize = request.PageSize ?? 10,
            };
            var validationResult = await validator.ValidateAsync(sanitizedRequest, cancellationToken);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

            if (!Guid.TryParse(request.AssignedAssetId, out Guid assignedAssetId)) return Errors.InvalidAssignedAssetId;

            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;
            if (!user.Roles.Contains(ApplicationRole.Admin)) return Errors.NotAdminUser;
            if (!Guid.TryParse(user.LocationId, out Guid userLocationId)) return Errors.LocationNotFound;

            var filterSpec = new FilterSpec(request, userLocationId);
            var spec = new PagingAndSortingSpec(request, userLocationId);

            var totalItems = await repo.CountAsync(filterSpec, cancellationToken);
            var items = await repo.ListAsync(spec, cancellationToken);
            return PagedList.Create(items, totalItems, sanitizedRequest.PageNumber.Value, sanitizedRequest.PageSize.Value);
        }
    }
}
