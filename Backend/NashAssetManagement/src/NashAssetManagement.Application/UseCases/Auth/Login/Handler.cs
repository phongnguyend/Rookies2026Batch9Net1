using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.Abstractions.Jwt;
using NashAssetManagement.Domain.Entities.Auth;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Auth.Login
{
    public class Handler(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenProvider jwtTokenProvider,
        IUnitOfWork uow,
        ILogger<Handler> logger,
        IDateTimeProvider dateTimeProvider,
        IRepository<RefreshToken, Guid> rfTokenRepository,
        IValidator<Request> validator) : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(Request orgReq, CancellationToken cancellationToken)
        {
            // Pre-cleaning
            var request = orgReq with
            {
                Username = orgReq.Username.Trim(),
                Password = orgReq.Password.Trim()
            };

            // Validation
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Find by username
            var user = await userManager.FindByNameAsync(request.Username);
            if (user is null)
            {
                return Errors.InvalidCredentials;
            }

            // Check locked out
            var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
            if (signInResult.IsLockedOut)
            {
                return Errors.UserLocked;
            }

            // Check remaining attempts
            if (!signInResult.Succeeded)
            {
                var failedCount = await userManager.GetAccessFailedCountAsync(user);
                var maxAttempts = Domain.Constants.IdentityConstants.MaxFailedAttempts;
                var remainingAttempts = Math.Max(0, maxAttempts - failedCount);
                return Errors.InvalidCredentialsWithRemainingAttempts(remainingAttempts);
            }

            // Compile with Single Session Refresh Token
            var activeRefreshTokens = rfTokenRepository.GetQueryableSet()
                                        .Where(x => x.UserId == user.Id && !x.IsRevoked && x.ExpiresAtUtc > dateTimeProvider.UtcNow);

            foreach (var token in activeRefreshTokens)
            {
                token.IsRevoked = true;
                token.RevokedAtUtc = dateTimeProvider.UtcNow;
            }

            // Generate tokens
            var roles = await userManager.GetRolesAsync(user);
            var accessToken = jwtTokenProvider.GenerateAccessToken(user, roles);
            var refreshToken = jwtTokenProvider.GenerateRefreshToken(user);

            try
            {
                await rfTokenRepository.AddAsync(refreshToken, cancellationToken);
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save refresh token for user {UserId}", user.Id);
                return Errors.PersistenceFailed;
            }

            return new Response(accessToken, refreshToken.Id.ToString());
        }
    }
}