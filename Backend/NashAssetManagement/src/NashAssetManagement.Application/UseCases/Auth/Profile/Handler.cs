using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Abstractions.AppIdentity;

namespace NashAssetManagement.Application.UseCases.Auth.Profile
{
    public class Handler(ICurrentUser currentUser)
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

            var locationId = Guid.TryParse(currentUser.LocationId, out var locId) ? locId : Guid.Empty;

            var response = new Response(
                currentUser.UserId.Value,
                currentUser.Username ?? string.Empty,
                locationId,
                currentUser.IsFirstTimeLogin,
                currentUser.Roles);

            return response;
        }
    }
}
