using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Domain.Entities.Auth;

namespace NashAssetManagement.Application.UseCases.Auth.Logout
{
    public class Handler(
        IUnitOfWork uow,
        ILogger<Handler> logger,
        IDateTimeProvider dateTimeProvider,
        ICurrentUser currentUser,
        IRepository<RefreshToken, Guid> refreshTokenRepository
    ) : IRequestHandler<Request, ErrorOr<Deleted>>
    {
        public async Task<ErrorOr<Deleted>> Handle(Request request, CancellationToken cancellationToken)
        {
            // authenticated user
            if (currentUser == null || !currentUser.UserId.HasValue)
            {
                // user is not authenticated, return deleted
                return Result.Deleted;
            }

            var userId = currentUser.UserId.Value;
            var now = dateTimeProvider.UtcNow;

            // find refresh token
            var refreshToken = refreshTokenRepository
                .GetQueryableSet()
                .FirstOrDefault(x =>
                    x.UserId == userId &&
                    !x.IsRevoked &&
                    x.ExpiresAtUtc > now);

            if (refreshToken is null)
            {
                logger.LogInformation(
                    "Logout completed: no active refresh token found for user {UserId}.",
                    userId);

                return Result.Deleted;
            }

            // revoke refresh token
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAtUtc = now;

            try
            {
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to revoke refresh token during logout for user {UserId}.",
                    userId);

                return Errors.UnexpectedError;
            }

            return Result.Deleted;
        }
    }
}