using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.Abstractions.Jwt;
using NashAssetManagement.Domain.Entities.Auth;
using NashAssetManagement.Domain.Entities.Identity;
using Microsoft.Extensions.Logging;

namespace NashAssetManagement.Application.UseCases.Auth.Refresh
{
    public class Handler(
        IRepository<RefreshToken, Guid> refreshTokenRepository,
        UserManager<User> userManager,
        IJwtTokenProvider jwtTokenProvider,
        IDateTimeProvider dateTimeProvider,
        ILogger<Handler> logger,
        IUnitOfWork uow)
        : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Errors.MissingRefreshToken;
            }

            if (!Guid.TryParse(request.RefreshToken, out var refreshTokenId))
            {
                return Errors.InvalidRefreshToken;
            }

            // Find refresh token
            var refreshToken =
                await refreshTokenRepository
                    .GetQueryableSet()
                    .FirstOrDefaultAsync(x => x.Id == refreshTokenId, cancellationToken);

            if (refreshToken is null)
            {
                return Errors.InvalidRefreshToken;
            }

            // Already revoked, may be because user login elsewhere cause single session refresh token
            if (refreshToken.IsRevoked)
            {
                return Errors.RevokedRefreshToken;
            }

            // Expired
            if (refreshToken.ExpiresAtUtc <= dateTimeProvider.UtcNow)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAtUtc = dateTimeProvider.UtcNow;
                await uow.SaveChangesAsync(cancellationToken);
                return Errors.ExpiredRefreshToken;
            }

            // User is deleted or removed from the database
            var user = await userManager.FindByIdAsync(refreshToken.UserId.ToString());
            if (user is null || user.IsDeleted)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAtUtc = dateTimeProvider.UtcNow;
                await uow.SaveChangesAsync(cancellationToken);
                return Errors.InvalidRefreshToken;
            }

            // Revoke old token
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAtUtc = dateTimeProvider.UtcNow;

            // Generate new access token
            var roles = await userManager.GetRolesAsync(user);
            var accessToken = jwtTokenProvider.GenerateAccessToken(user, roles);

            // Generate new refresh token
            var newRefreshToken = jwtTokenProvider.GenerateRefreshToken(user);

            try
            {
                // Save new refresh token
                await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to rotate refresh token for user {UserId}: {UserName}", user.Id, user.UserName);
                return Errors.PersistenceFailed;
            }

            return new Response(accessToken, newRefreshToken.Id.ToString());
        }
    }
}
