using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.Abstractions.Jwt;
using NashAssetManagement.Domain.Entities.Auth;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Auth.FirstChangePassword
{
    public class Handler(
        UserManager<User> userManager,
        ICurrentUser currentUser,
        IValidator<Request> validator,
        IUnitOfWork uow,
        IJwtTokenProvider jwtTokenProvider,
        IRepository<RefreshToken, Guid> rfTokenRepository,
        IDateTimeProvider dateTimeProvider,
        ILogger<Handler> logger)
        : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(
            Request orgReq,
            CancellationToken cancellationToken)
        {
            // Pre-cleaning
            var request = orgReq with
            {
                NewPassword = orgReq.NewPassword.Trim()
            };

            // Validation
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Only authenticated user can change its own password
            if (currentUser == null || !currentUser.IsAuthenticated || currentUser.UserId == null)
            {
                return Errors.Forbidden;
            }

            // Check if user not found or disabled
            var user = await userManager.FindByIdAsync(currentUser.UserId.ToString() ?? string.Empty);
            if (user is null || user.IsDeleted)
            {
                return Errors.UserNotFound;
            }

            // Check if first login or not 
            if (!user.IsFirstLogin)
            {
                return Errors.NotFirstLogin;
            }

            string accessToken;
            string refreshTokenId;

            try
            {
                // Change password without needing current password
                var removeResult = await userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    var errorDescription = string.Join(" ", removeResult.Errors.Select(e => e.Description));
                    return Error.Validation(
                        code: "FirstChangePassword.RemoveFailed",
                        description: errorDescription);
                }

                var addResult = await userManager.AddPasswordAsync(user, request.NewPassword);
                if (!addResult.Succeeded)
                {
                    var errorDescription = string.Join(" ", addResult.Errors.Select(e => e.Description));
                    return Error.Validation(
                        code: "FirstChangePassword.AddFailed",
                        description: errorDescription);
                }

                // Mark as no longer first login
                user.IsFirstLogin = false;

                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return Errors.ChangePasswordFailed;
                }

                // Compile with Single Session Refresh Token - revoke active refresh tokens
                var activeRefreshTokens = rfTokenRepository.GetQueryableSet()
                                            .Where(x => x.UserId == user.Id && !x.IsRevoked && x.ExpiresAtUtc > dateTimeProvider.UtcNow);

                foreach (var token in activeRefreshTokens)
                {
                    token.IsRevoked = true;
                    token.RevokedAtUtc = dateTimeProvider.UtcNow;
                }

                // Generate new tokens (now with IsFirstLogin = false claim!)
                var roles = await userManager.GetRolesAsync(user);
                accessToken = jwtTokenProvider.GenerateAccessToken(user, roles);
                var refreshToken = jwtTokenProvider.GenerateRefreshToken(user);
                refreshTokenId = refreshToken.Id.ToString();

                await rfTokenRepository.AddAsync(refreshToken, cancellationToken);
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to change password and refresh token for user {UserId}", user.Id);
                return Errors.PersistenceFailed;
            }

            return new Response(accessToken, refreshTokenId);
        }
    }
}
