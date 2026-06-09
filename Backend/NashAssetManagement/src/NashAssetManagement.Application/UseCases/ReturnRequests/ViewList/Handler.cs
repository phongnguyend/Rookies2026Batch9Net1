using FluentValidation;
using MediatR;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Entities.Core;
using ErrorOr;
using NashAssetManagement.Application.Utilities;
using System.Text.RegularExpressions;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.ViewList
{
    internal class Handler(
        IRepository<ReturnRequest, Guid> repository,
        ICurrentUser currentUser,
        IValidator<Request> validator
    )
        : IRequestHandler<Request, ErrorOr<PagedList<Response>>>
    {
        public async Task<ErrorOr<PagedList<Response>>> Handle(Request request, CancellationToken cancellationToken)
        {
            var cleanedRequest = request with
            {
              SearchTerm = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? request.SearchTerm
                : NormalizeSearchTerm(request.SearchTerm),
              SortBy = request.SortBy?.Trim(),
              PageNumber = request.PageNumber ?? 1,
              PageSize = request.PageSize ?? 20  
            };
            var validationResults = await validator.ValidateAsync(cleanedRequest, cancellationToken);
            if (!validationResults.IsValid)
                throw new ValidationException(validationResults.Errors);

            if (currentUser.UserId == null)
                return Errors.Unauthorized;

            if (string.IsNullOrEmpty(currentUser.LocationId))
                return Errors.UserHasNoLocation;

            var filterSpec = new FilterSpec(cleanedRequest, currentUser.LocationId);
            var spec = new PagingAndSortingSpec(cleanedRequest, currentUser.LocationId);

            var totalItems = await repository.CountAsync(filterSpec, cancellationToken);
            var items = await repository.ListAsync(spec, cancellationToken);
            return PagedList.Create(items, totalItems, cleanedRequest.PageNumber.Value, cleanedRequest.PageSize.Value);
        }

        private static string NormalizeSearchTerm(string searchTerm)
        {
            return Regex.Replace(searchTerm.Trim(), @"\s+", " ");
        }
    }
}
