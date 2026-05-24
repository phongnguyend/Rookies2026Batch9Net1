using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.UseCases.Auth.ChangePassword;
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

        [Fact]
        public async Task Handle_Should_Return_UserNotFound_When_User_Does_Not_Exist()
        {
            var userManagerMock = CreateUserManagerMock();
            var loggerMock = new Mock<ILogger<Handler>>();
            var validator = new Validator();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                validator);

            var request = new Request("OldPassword123", "NewPassword123");

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(Errors.UserNotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_Should_Return_IncorrectOldPassword_When_Password_Is_Wrong()
        {
            var userManagerMock = CreateUserManagerMock();
            var loggerMock = new Mock<ILogger<Handler>>();
            var validator = new Validator();

            var user = new User();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(x => x.ChangePasswordAsync(
                    user,
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "PasswordMismatch"
                        }));

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                validator);

            var request = new Request("WrongPassword", "NewPassword123");

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(Errors.IncorrectOldPassword.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_Should_Return_Success_When_Password_Is_Changed()
        {
            var userManagerMock = CreateUserManagerMock();
            var loggerMock = new Mock<ILogger<Handler>>();
            var validator = new Validator();

            var user = new User();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(x => x.ChangePasswordAsync(
                    user,
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var handler = new Handler(
                loggerMock.Object,
                userManagerMock.Object,
                validator);

            var request = new Request("OldPassword123", "NewPassword123");

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(
                "Your password has been changed successfully!",
                result.Value.Message);
        }
    }
}
