using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Auth.Logout;
using NashAssetManagement.Domain.Entities.Auth;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Auth.Logout
{
    public class HandlerTests
    {
        [Fact]
        public async Task Handle_UserIsNotAuthenticated_ShouldReturnUnauthorized()
        {
            // Arrange
            var uowMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<Handler>>();
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var currentUserMock = new Mock<ICurrentUser>();
            var refreshTokenRepositoryMock = new Mock<IRepository<RefreshToken, Guid>>();

            currentUserMock
                .Setup(x => x.UserId)
                .Returns((Guid?)null);

            var handler = new Handler(
                uowMock.Object,
                loggerMock.Object,
                dateTimeProviderMock.Object,
                currentUserMock.Object,
                refreshTokenRepositoryMock.Object);

            // Act
            var result = await handler.Handle(new Request(), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Unauthorized.Code, result.FirstError.Code);

            refreshTokenRepositoryMock.Verify(
                x => x.GetQueryableSet(),
                Times.Never);

            uowMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ActiveRefreshTokenDoesNotExist_ShouldReturnInvalidRefreshToken()
        {
            // Arrange
            var uowMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<Handler>>();
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var currentUserMock = new Mock<ICurrentUser>();
            var refreshTokenRepositoryMock = new Mock<IRepository<RefreshToken, Guid>>();

            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            currentUserMock
                .Setup(x => x.UserId)
                .Returns(userId);

            dateTimeProviderMock
                .Setup(x => x.UtcNow)
                .Returns(now);

            refreshTokenRepositoryMock
                .Setup(x => x.GetQueryableSet())
                .Returns(new List<RefreshToken>().AsQueryable());

            var handler = new Handler(
                uowMock.Object,
                loggerMock.Object,
                dateTimeProviderMock.Object,
                currentUserMock.Object,
                refreshTokenRepositoryMock.Object);

            // Act
            var result = await handler.Handle(new Request(), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidRefreshToken.Code, result.FirstError.Code);

            uowMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_RefreshTokenIsExpired_ShouldReturnInvalidRefreshToken()
        {
            // Arrange
            var uowMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<Handler>>();
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var currentUserMock = new Mock<ICurrentUser>();
            var refreshTokenRepositoryMock = new Mock<IRepository<RefreshToken, Guid>>();

            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ExpiresAtUtc = now.AddMinutes(-1),
                IsRevoked = false,
                RevokedAtUtc = null
            };

            currentUserMock
                .Setup(x => x.UserId)
                .Returns(userId);

            dateTimeProviderMock
                .Setup(x => x.UtcNow)
                .Returns(now);

            refreshTokenRepositoryMock
                .Setup(x => x.GetQueryableSet())
                .Returns(new List<RefreshToken> { refreshToken }.AsQueryable());

            var handler = new Handler(
                uowMock.Object,
                loggerMock.Object,
                dateTimeProviderMock.Object,
                currentUserMock.Object,
                refreshTokenRepositoryMock.Object);

            // Act
            var result = await handler.Handle(new Request(), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidRefreshToken.Code, result.FirstError.Code);

            uowMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_RefreshTokenIsAlreadyRevoked_ShouldReturnInvalidRefreshToken()
        {
            // Arrange
            var uowMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<Handler>>();
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var currentUserMock = new Mock<ICurrentUser>();
            var refreshTokenRepositoryMock = new Mock<IRepository<RefreshToken, Guid>>();

            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ExpiresAtUtc = now.AddMinutes(30),
                IsRevoked = true,
                RevokedAtUtc = now.AddMinutes(-5)
            };

            currentUserMock
                .Setup(x => x.UserId)
                .Returns(userId);

            dateTimeProviderMock
                .Setup(x => x.UtcNow)
                .Returns(now);

            refreshTokenRepositoryMock
                .Setup(x => x.GetQueryableSet())
                .Returns(new List<RefreshToken> { refreshToken }.AsQueryable());

            var handler = new Handler(
                uowMock.Object,
                loggerMock.Object,
                dateTimeProviderMock.Object,
                currentUserMock.Object,
                refreshTokenRepositoryMock.Object);

            // Act
            var result = await handler.Handle(new Request(), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidRefreshToken.Code, result.FirstError.Code);

            uowMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ValidActiveRefreshToken_ShouldRevokeRefreshTokenAndReturnDeleted()
        {
            // Arrange
            var uowMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<Handler>>();
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var currentUserMock = new Mock<ICurrentUser>();
            var refreshTokenRepositoryMock = new Mock<IRepository<RefreshToken, Guid>>();

            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ExpiresAtUtc = now.AddMinutes(30),
                IsRevoked = false,
                RevokedAtUtc = null
            };

            currentUserMock
                .Setup(x => x.UserId)
                .Returns(userId);

            dateTimeProviderMock
                .Setup(x => x.UtcNow)
                .Returns(now);

            refreshTokenRepositoryMock
                .Setup(x => x.GetQueryableSet())
                .Returns(new List<RefreshToken> { refreshToken }.AsQueryable());

            uowMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var handler = new Handler(
                uowMock.Object,
                loggerMock.Object,
                dateTimeProviderMock.Object,
                currentUserMock.Object,
                refreshTokenRepositoryMock.Object);

            // Act
            var result = await handler.Handle(new Request(), CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Deleted, result.Value);

            Assert.True(refreshToken.IsRevoked);
            Assert.Equal(now, refreshToken.RevokedAtUtc);

            uowMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_SaveChangesThrowsException_ShouldReturnUnexpectedError()
        {
            // Arrange
            var uowMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<Handler>>();
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var currentUserMock = new Mock<ICurrentUser>();
            var refreshTokenRepositoryMock = new Mock<IRepository<RefreshToken, Guid>>();

            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ExpiresAtUtc = now.AddMinutes(30),
                IsRevoked = false,
                RevokedAtUtc = null
            };

            currentUserMock
                .Setup(x => x.UserId)
                .Returns(userId);

            dateTimeProviderMock
                .Setup(x => x.UtcNow)
                .Returns(now);

            refreshTokenRepositoryMock
                .Setup(x => x.GetQueryableSet())
                .Returns(new List<RefreshToken> { refreshToken }.AsQueryable());

            uowMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var handler = new Handler(
                uowMock.Object,
                loggerMock.Object,
                dateTimeProviderMock.Object,
                currentUserMock.Object,
                refreshTokenRepositoryMock.Object);

            // Act
            var result = await handler.Handle(new Request(), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnexpectedError.Code, result.FirstError.Code);

            Assert.True(refreshToken.IsRevoked);
            Assert.Equal(now, refreshToken.RevokedAtUtc);

            uowMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}