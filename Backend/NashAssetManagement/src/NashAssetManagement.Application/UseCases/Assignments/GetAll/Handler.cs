using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Assignments.GetAll
{
    internal class Handler(IRepository<Assignment, Guid> repo, ICurrentUser currentUser, UserManager<User> userManager, IValidator<Query> validator)
    : IRequestHandler<Query, ErrorOr<PagedList<Response>>>
    {
        public async Task<ErrorOr<PagedList<Response>>> Handle(
            Query query,
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await userManager.FindByIdAsync(currentUser.UserId.ToString());
            if (user is null || user.IsDeleted)
            {
                return Errors.UserNotFound;
            }

            var filterSpec = new FilterSpec(query, user.LocationId);
            var pagingSpec = new PagingSpec(query, user.LocationId);

            var totalItems = await repo.CountAsync(filterSpec, cancellationToken);
            var items = await repo.ListAsync(pagingSpec, cancellationToken);

            return PagedList.Create(items, totalItems, query.PageNumber ?? 1, query.PageSize ?? 10);
        }
    }
}
