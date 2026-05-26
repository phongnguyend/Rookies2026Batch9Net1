using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.UseCases.Auth.ChangePassword;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Identity;
using Xunit;

// Naming convention: MethodName_StateUnderTest_ExpectedBehavior
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
        public async Task ChangePasswordHandler_CurrentUserIdIsNull_ShouldReturnUserNotFound()
        {
            var userManagerMock = CreateUserManagerMock();
            var loggerMock = new Mock<ILogger<Handler>>();
            var validator = new Validator();
            var currentUserMock = CreateCurrentUserMock(null);

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                currentUserMock.Object,
                validator);

            var request = new Request("OldPassword123!", "NewPassword123!");

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(Errors.UserNotFound.Code, result.FirstError.Code);

            userManagerMock.Verify(
                x => x.FindByIdAsync(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ChangePasswordHandler_UserDoesNotExist_ShouldReturnUserNotFound()
        {
            var userManagerMock = CreateUserManagerMock();
            var loggerMock = new Mock<ILogger<Handler>>();
            var validator = new Validator();

            var userId = Guid.NewGuid();
            var currentUserMock = CreateCurrentUserMock(userId);

            userManagerMock
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User?)null);

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                currentUserMock.Object,
                validator);

            var request = new Request("OldPassword123!", "NewPassword123!");

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(Errors.UserNotFound.Code, result.FirstError.Code);

            userManagerMock.Verify(
                x => x.FindByIdAsync(userId.ToString()),
                Times.Once);

            userManagerMock.Verify(
                x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()),
                Times.Never);

            userManagerMock.Verify(
                x => x.ChangePasswordAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ChangePasswordHandler_IncorrectOldPassword_ShouldReturnIncorrectOldPassword()
        {
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
                .Setup(x => x.CheckPasswordAsync(user, "WrongPassword123!"))
                .ReturnsAsync(false);

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                currentUserMock.Object,
                validator);

            var request = new Request("WrongPassword123!", "NewPassword123!");

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(Errors.IncorrectOldPassword.Code, result.FirstError.Code);

            userManagerMock.Verify(
                x => x.CheckPasswordAsync(user, "WrongPassword123!"),
                Times.Once);

            userManagerMock.Verify(
                x => x.ChangePasswordAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ChangePasswordHandler_ChangePasswordFails_ShouldReturnChangePasswordFailed()
        {
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
                .Setup(x => x.CheckPasswordAsync(user, "OldPassword123!"))
                .ReturnsAsync(true);

            userManagerMock
                .Setup(x => x.ChangePasswordAsync(
                    user,
                    "OldPassword123!",
                    "NewPassword123!"))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "ChangePasswordFailed",
                            Description = "Password change failed"
                        }));

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                currentUserMock.Object,
                validator);

            var request = new Request("OldPassword123!", "NewPassword123!");

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(Errors.ChangePasswordFailed.Code, result.FirstError.Code);

            userManagerMock.Verify(
                x => x.ChangePasswordAsync(
                    user,
                    "OldPassword123!",
                    "NewPassword123!"),
                Times.Once);
        }

        [Fact]
        public async Task ChangePasswordHandler_ValidRequest_ShouldReturnSuccess()
        {
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
                .Setup(x => x.CheckPasswordAsync(user, "OldPassword123!"))
                .ReturnsAsync(true);

            userManagerMock
                .Setup(x => x.ChangePasswordAsync(
                    user,
                    "OldPassword123!",
                    "NewPassword123!"))
                .ReturnsAsync(IdentityResult.Success);

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                currentUserMock.Object,
                validator);

            var request = new Request("OldPassword123!", "NewPassword123!");

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            userManagerMock.Verify(
                x => x.FindByIdAsync(userId.ToString()),
                Times.Once);

            userManagerMock.Verify(
                x => x.CheckPasswordAsync(user, "OldPassword123!"),
                Times.Once);

            userManagerMock.Verify(
                x => x.ChangePasswordAsync(
                    user,
                    "OldPassword123!",
                    "NewPassword123!"),
                Times.Once);
        }

        [Fact]
        public async Task ChangePasswordHandler_ChangePasswordThrowsException_ShouldReturnUnexpectedError()
        {
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
                .Setup(x => x.CheckPasswordAsync(user, "OldPassword123!"))
                .ReturnsAsync(true);

            userManagerMock
                .Setup(x => x.ChangePasswordAsync(
                    user,
                    "OldPassword123!",
                    "NewPassword123!"))
                .ThrowsAsync(new Exception("Unexpected error"));

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                currentUserMock.Object,
                validator);

            var request = new Request("OldPassword123!", "NewPassword123!");

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(Errors.UnexpectedError.Code, result.FirstError.Code);
        }
    }
}
