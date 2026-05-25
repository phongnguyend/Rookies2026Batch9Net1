using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Auth.Profile
{
    public class Handler(ICurrentUser currentUser, UserManager<User> userManager)
        : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            {
                return Errors.UserNotFound;
            }

            var user = await userManager.FindByIdAsync(currentUser.UserId.Value.ToString());
            if (user is null || user.IsDeleted)
            {
                return Errors.UserNotFound;
            }

            var locationId = Guid.TryParse(currentUser.LocationId, out var locId) ? locId : Guid.Empty;

            var response = new Response(
                user.Id,
                user.UserName ?? string.Empty,
                locationId,
                user.IsFirstLogin,
                currentUser.Roles);

            return response;
        }
    }
}
