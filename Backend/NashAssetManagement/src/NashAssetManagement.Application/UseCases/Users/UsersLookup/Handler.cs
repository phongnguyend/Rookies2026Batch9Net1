using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Users.UsersLookup
{
    internal class Handler(
        UserManager<User> userManager,
        ICurrentUser user,
        IValidator<Request> validator)
        : IRequestHandler<Request, ErrorOr<PagedList<Response>>>
    {
        const string StaffCodeSortOption = "staffcode";
        const string FullNameSortOption = "fullname";
        const string TypeSortOption = "type";

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

            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;
            if (!user.Roles.Contains(ApplicationRole.Admin)) return Errors.NotAdminUser;
            if (!Guid.TryParse(user.LocationId, out Guid userLocationId)) return Errors.LocationNotFound;

            IQueryable<User> query = userManager.Users;
            query = query.Where(x => x.LocationId == userLocationId && !x.IsDeleted);
            if (!string.IsNullOrWhiteSpace(sanitizedRequest.SearchTerm))
            {
                var searchTerm = sanitizedRequest.SearchTerm.Trim();
                query = query.Where(x => x.StaffCode.Contains(searchTerm)
                    || x.FirstName.Contains(searchTerm)
                    || x.LastName.Contains(searchTerm));
            }

            int totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(query, sanitizedRequest);
            query = ApplyPaging(query, sanitizedRequest);
            IQueryable<Response> translatedQuery = query.AsNoTracking()
                .Select(x => new Response(
                    x.Id,
                    x.StaffCode,
                    $"{x.FirstName} {x.LastName}",
                    x.UserType.ToString()));

            var items = await translatedQuery.ToListAsync(cancellationToken);
            return PagedList.Create(items, totalCount, request.PageNumber ?? 1, request.PageSize ?? 10);
        }

        private IQueryable<User> ApplySorting(IQueryable<User> query, Request request)
        {
            bool descending = request.SortDesc ?? false;
            return request.SortBy?.Trim().ToLowerInvariant() switch
            {
                StaffCodeSortOption => descending
                    ? query.OrderByDescending(x => x.StaffCode)
                    : query.OrderBy(x => x.StaffCode),
                FullNameSortOption => descending
                    ? query.OrderByDescending(x => x.FirstName).ThenByDescending(x => x.LastName)
                    : query.OrderBy(x => x.FirstName).ThenBy(x => x.LastName),
                TypeSortOption => descending
                    ? query.OrderByDescending(x => x.UserType)
                    : query.OrderBy(x => x.UserType),
                _ => descending
                    ? query.OrderByDescending(x => x.FirstName).ThenByDescending(x => x.LastName)
                    : query.OrderBy(x => x.FirstName).ThenBy(x => x.LastName),
            };
        }

        private IQueryable<User> ApplyPaging(IQueryable<User> query, Request request)
        {
            int pageNumber = request.PageNumber ?? 1;
            int pageSize = request.PageSize ?? 10;
            int skipAmount = (pageNumber - 1) * pageSize;
            return query.Skip(skipAmount).Take(pageSize);
        }
    }
}
