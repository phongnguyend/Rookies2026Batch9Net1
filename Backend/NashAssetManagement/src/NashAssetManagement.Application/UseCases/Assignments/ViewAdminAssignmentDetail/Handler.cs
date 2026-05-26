using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Assignments.GetById
{
    internal class Handler(IRepository<Assignment, Guid> repo, ICurrentUser currentUser, UserManager<User> userManager)
    : IRequestHandler<Query, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(
            Query query,
            CancellationToken cancellationToken)
        {

            if (!currentUser.IsAuthenticated) return Errors.UnauthorizedUser;
            var userId = currentUser.UserId;
            if (userId == null) return Errors.UnidentifiedUser;

            var user = await userManager.FindByIdAsync(userId.Value.ToString());
            if (user is null || user.IsDeleted)
            {
                return Errors.UserNotFound;
            }

            var result = await repo.FirstOrDefaultAsync(new Spec(query, user.LocationId), cancellationToken);

            if (result is null)
                return Errors.AssignmentNotFoundWithId(query.Id);

            return result;
        }
    }
}
