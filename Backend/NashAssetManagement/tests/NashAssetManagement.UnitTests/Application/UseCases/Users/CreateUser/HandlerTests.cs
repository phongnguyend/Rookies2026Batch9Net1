using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.UseCases.Users.CreateUser;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using Xunit;

// Naming convention: MethodName_StateUnderTest_ExpectedBehavior
namespace NashAssetManagement.UnitTests.Application.UseCases.Users.CreateUser
{
    public class HandlerTests
    {
        private static Mock<UserManager<User>> CreateUserManagerMock(List<User>? users = null)
        {
            var store = new Mock<IUserStore<User>>();

            var userManagerMock = new Mock<UserManager<User>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);

            if (users is not null)
            {
                var mockUsers = users.BuildMockDbSet();

                userManagerMock
                    .Setup(x => x.Users)
                    .Returns(mockUsers.Object);
            }

            return userManagerMock;
        }

        private static Mock<ICurrentUser> CreateCurrentUserMock(Guid? userId)
        {
            var currentUserMock = new Mock<ICurrentUser>();

            currentUserMock
                .Setup(x => x.UserId)
                .Returns(userId);

            return currentUserMock;
        }

        private static Request ValidRequest()
        {
            return new Request(
                FirstName: "Binh",
                LastName: "Nguyen Van",
                DayOfBirth: DateTime.UtcNow.Date.AddYears(-20),
                JoinedDate: GetNextWorkingDay(DateTime.UtcNow.Date),
                Gender: Gender.Male,
                UserType: UserType.Staff
            );
        }

        private static DateTime GetNextWorkingDay(DateTime date)
        {
            var nextDate = date;

            while (nextDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                nextDate = nextDate.AddDays(1);
            }

            return nextDate;
        }

        [Fact]
        public async Task CreateUserHandler_CurrentUserIdIsNull_ShouldReturnUserNotFound()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock(new List<User>());
            var currentUserMock = CreateCurrentUserMock(null);
            var validator = new Validators();
            var loggerMock = new Mock<ILogger<Handler>>();

            var handler = new Handler(
                userManagerMock.Object,
                currentUserMock.Object,
                validator,
                loggerMock.Object);

            var request = ValidRequest();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UserNotFound.Code, result.FirstError.Code);

            userManagerMock.Verify(
                x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateUserHandler_AdminDoesNotExist_ShouldReturnUserNotFound()
        {
            // Arrange
            var adminId = Guid.NewGuid();

            var users = new List<User>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    StaffCode = "SD0001",
                    UserName = "existinguser"
                }
            };

            var userManagerMock = CreateUserManagerMock(users);
            var currentUserMock = CreateCurrentUserMock(adminId);
            var validator = new Validators();
            var loggerMock = new Mock<ILogger<Handler>>();

            var handler = new Handler(
                userManagerMock.Object,
                currentUserMock.Object,
                validator,
                loggerMock.Object);

            var request = ValidRequest();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UserNotFound.Code, result.FirstError.Code);

            userManagerMock.Verify(
                x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateUserHandler_InvalidRequest_ShouldThrowValidationException()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock(new List<User>());
            var currentUserMock = CreateCurrentUserMock(Guid.NewGuid());
            var validator = new Validators();
            var loggerMock = new Mock<ILogger<Handler>>();

            var handler = new Handler(
                userManagerMock.Object,
                currentUserMock.Object,
                validator,
                loggerMock.Object);

            var request = ValidRequest() with { FirstName = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                handler.Handle(request, CancellationToken.None));

            userManagerMock.Verify(
                x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateUserHandler_CreateUserFails_ShouldReturnCreateUserFailed()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            var users = new List<User>
            {
                new()
                {
                    Id = adminId,
                    StaffCode = "SD0001",
                    UserName = "admin",
                    LocationId = locationId
                }
            };

            var userManagerMock = CreateUserManagerMock(users);
            var currentUserMock = CreateCurrentUserMock(adminId);
            var validator = new Validators();
            var loggerMock = new Mock<ILogger<Handler>>();

            userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>()))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "CreateUserFailed",
                            Description = "Create user failed"
                        }));

            var handler = new Handler(
                userManagerMock.Object,
                currentUserMock.Object,
                validator,
                loggerMock.Object);

            var request = ValidRequest();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CreateUserFailed.Code, result.FirstError.Code);

            userManagerMock.Verify(
                x => x.CreateAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateUserHandler_CreateUserThrowsException_ShouldReturnUnexpectedError()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            var users = new List<User>
            {
                new()
                {
                    Id = adminId,
                    StaffCode = "SD0001",
                    UserName = "admin",
                    LocationId = locationId
                }
            };

            var userManagerMock = CreateUserManagerMock(users);
            var currentUserMock = CreateCurrentUserMock(adminId);
            var validator = new Validators();
            var loggerMock = new Mock<ILogger<Handler>>();

            userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var handler = new Handler(
                userManagerMock.Object,
                currentUserMock.Object,
                validator,
                loggerMock.Object);

            var request = ValidRequest();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnexpectedError.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task CreateUserHandler_ValidRequest_ShouldReturnSuccess()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            var users = new List<User>
            {
                new()
                {
                    Id = adminId,
                    StaffCode = "SD0001",
                    UserName = "admin",
                    LocationId = locationId
                }
            };

            var userManagerMock = CreateUserManagerMock(users);
            var currentUserMock = CreateCurrentUserMock(adminId);
            var validator = new Validators();
            var loggerMock = new Mock<ILogger<Handler>>();

            User? createdUser = null;
            string? createdPassword = null;

            userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>()))
                .Callback<User, string>((user, password) =>
                {
                    createdUser = user;
                    createdPassword = password;
                })
                .ReturnsAsync(IdentityResult.Success);

            var handler = new Handler(
                userManagerMock.Object,
                currentUserMock.Object,
                validator,
                loggerMock.Object);

            var request = ValidRequest();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);

            Assert.NotNull(createdUser);
            Assert.Equal("SD0002", createdUser!.StaffCode);
            Assert.Equal("binhnv", createdUser.UserName);
            Assert.Equal(locationId, createdUser.LocationId);
            Assert.Equal("Binh", createdUser.FirstName);
            Assert.Equal("Nguyen Van", createdUser.LastName);
            Assert.Equal(request.DayOfBirth, createdUser.DateOfBirth);
            Assert.Equal(request.UserType, createdUser.UserType);
            Assert.Equal(request.Gender, createdUser.Gender);
            Assert.True(createdUser.IsFirstLogin);
            Assert.Equal($"binhnv@{CompanyConstants.EmailDomain}", createdUser.Email);

            Assert.Equal($"binhnv@{request.DayOfBirth:ddMMyyyy}", createdPassword);

            Assert.Equal(createdUser.Id, result.Value.Id);
            Assert.Equal("SD0002", result.Value.StaffCode);
            Assert.Equal("binhnv", result.Value.UserName);
        }

        [Fact]
        public async Task CreateUserHandler_UsernameAlreadyExists_ShouldGenerateUsernameWithNextIndex()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            var users = new List<User>
            {
                new()
                {
                    Id = adminId,
                    StaffCode = "SD0001",
                    UserName = "admin",
                    LocationId = locationId
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    StaffCode = "SD0002",
                    UserName = "binhnv"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    StaffCode = "SD0003",
                    UserName = "binhnv1"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    StaffCode = "SD0004",
                    UserName = "binhnv2"
                }
            };

            var userManagerMock = CreateUserManagerMock(users);
            var currentUserMock = CreateCurrentUserMock(adminId);
            var validator = new Validators();
            var loggerMock = new Mock<ILogger<Handler>>();

            User? createdUser = null;
            string? createdPassword = null;

            userManagerMock
                .Setup(x => x.CreateAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>()))
                .Callback<User, string>((user, password) =>
                {
                    createdUser = user;
                    createdPassword = password;
                })
                .ReturnsAsync(IdentityResult.Success);

            var handler = new Handler(
                userManagerMock.Object,
                currentUserMock.Object,
                validator,
                loggerMock.Object);

            var request = ValidRequest();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);

            Assert.NotNull(createdUser);
            Assert.Equal("SD0005", createdUser!.StaffCode);
            Assert.Equal("binhnv3", createdUser.UserName);
            Assert.Equal($"binhnv3@{request.DayOfBirth:ddMMyyyy}", createdPassword);

            Assert.Equal("SD0005", result.Value.StaffCode);
            Assert.Equal("binhnv3", result.Value.UserName);
        }

    }
}