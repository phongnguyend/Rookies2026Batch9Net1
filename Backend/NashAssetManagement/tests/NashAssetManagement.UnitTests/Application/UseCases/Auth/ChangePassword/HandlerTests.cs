using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.UseCases.Auth.ChangePassword;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Identity;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Auth.ChangePassword
{
    public class HandlerTests
    {
        private static Mock<UserManager<User>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();

            return new Mock<UserManager<User>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
        }

        private static Mock<ICurrentUser> CreateCurrentUserMock(Guid? userId)
        {
            var currentUserMock = new Mock<ICurrentUser>();

            currentUserMock
                .Setup(x => x.UserId)
                .Returns(userId);

            return currentUserMock;
        }

        [Fact]
        public async Task Handle_Should_Return_UserNotFound_When_User_Does_Not_Exist()
        {
            // Arrage
            var userManagerMock = CreateUserManagerMock();
            var loggerMock = new Mock<ILogger<Handler>>();
            var validator = new Validator();

            var userId = Guid.NewGuid();
            var currentUserMock = CreateCurrentUserMock(userId);

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                currentUserMock.Object,
                validator);

            var request = new Request("OldPassword123", "NewPassword123");

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UserNotFound.Code, result.FirstError.Code);

            userManagerMock.Verify(
                x => x.FindByIdAsync(userId.ToString()),
                Times.Once);

            userManagerMock.Verify(
                x => x.ChangePasswordAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_IncorrectOldPassword_When_Password_Is_Wrong()
        {
            // Arrage
            var userManagerMock = CreateUserManagerMock();
            var loggerMock = new Mock<ILogger<Handler>>();
            var validator = new Validator();

            var user = new User();
            var userId = Guid.NewGuid();
            var currentUserMock = CreateCurrentUserMock(userId);
            
            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(x => x.ChangePasswordAsync(
                    user,
                    "WrongPassword",
                    "NewPassword123"))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = PasswordConstants.PasswordMismatchCode
                        }));

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                currentUserMock.Object,
                validator);

            var request = new Request("WrongPassword", "NewPassword123");

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.IncorrectOldPassword.Code, result.FirstError.Code);

            userManagerMock.Verify(
                x => x.ChangePasswordAsync(
                    user,
                    "WrongPassword",
                    "NewPassword123"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Success_When_Password_Is_Changed()
        {
            // Arrage
            var userManagerMock = CreateUserManagerMock();
            var loggerMock = new Mock<ILogger<Handler>>();
            var validator = new Validator();

            var user = new User();
            var userId = Guid.NewGuid();
            var currentUserMock = CreateCurrentUserMock(userId);

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(x => x.ChangePasswordAsync(
                    user,
                    "OldPassword123",
                    "NewPassword123"))
                .ReturnsAsync(IdentityResult.Success);

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                currentUserMock.Object,
                validator);

            var request = new Request("OldPassword123", "NewPassword123");

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(
                "Your password has been changed successfully!",
                result.Value.Message);

            userManagerMock.Verify(
                x => x.FindByIdAsync(userId.ToString()),
                Times.Once);

            userManagerMock.Verify(
                x => x.ChangePasswordAsync(
                    user,
                    "OldPassword123",
                    "NewPassword123"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_IdentityValidationErrors_When_ChangePassword_Fails_With_Other_Errors()
        {
            // Arrage
            var userManagerMock = CreateUserManagerMock();
            var loggerMock = new Mock<ILogger<Handler>>();
            var validator = new Validator();

            var user = new User();
            var userId = Guid.NewGuid();
            var currentUserMock = CreateCurrentUserMock(userId);

            userManagerMock
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(x => x.ChangePasswordAsync(
                    user,
                    "OldPassword123",
                    "123"))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "PasswordTooShort",
                            Description = "Passwords must be at least 6 characters."
                        }));

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                currentUserMock.Object,
                validator);

            var request = new Request("OldPassword123", "123");

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("PasswordTooShort", result.FirstError.Code);
            Assert.Equal(
                "Passwords must be at least 6 characters.",
                result.FirstError.Description);

            userManagerMock.Verify(
                x => x.ChangePasswordAsync(
                    user,
                    "OldPassword123",
                    "123"),
                Times.Once);            
        }
    }
}
