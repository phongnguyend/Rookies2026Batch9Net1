using Ardalis.Specification;
using Microsoft.AspNetCore.Identity;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assignments.GetById;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.ViewAdminAssignmentDetail
{
    public class HandlerTest
    {
        // ── Factories
        private static Mock<IRepository<Assignment, Guid>> CreateRepoMock() =>
            new Mock<IRepository<Assignment, Guid>>();

        private static Mock<UserManager<User>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private static Mock<ICurrentUser> CreateCurrentUserMock(
            bool isAuthenticated = true,
            Guid? userId = null)
        {
            var mock = new Mock<ICurrentUser>();
            mock.Setup(x => x.IsAuthenticated).Returns(isAuthenticated);
            mock.Setup(x => x.UserId).Returns(userId ?? Guid.NewGuid());
            return mock;
        }

        // ── Fixtures

        private static User CreateUser(Guid? locationId = null) =>
            new User { LocationId = locationId ?? Guid.NewGuid(), IsDeleted = false };

        private static Response CreateResponse(Guid? id = null) =>
            new Response(
                id ?? Guid.NewGuid(),
                "AS001",
                "Laptop Dell XPS",
                "Core i7, 16GB RAM",
                "userA",
                "admin",
                "01/01/2024",
                "Accepted",
                "Handle with care");

        private static Handler CreateHandler(
            Mock<IRepository<Assignment, Guid>> repo,
            Mock<ICurrentUser> currentUser,
            Mock<UserManager<User>> userManager) =>
            new Handler(repo.Object, currentUser.Object, userManager.Object);

        // Success cases

        [Fact]
        public async Task Handle_AssignmentExists_ReturnsResponse()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock();

            var assignmentId = Guid.NewGuid();
            var response = CreateResponse(assignmentId);

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(assignmentId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(assignmentId, result.Value.Id);
            Assert.Equal("AS001", result.Value.AssetCode);
            Assert.Equal("Laptop Dell XPS", result.Value.AssetName);
            Assert.Equal("Core i7, 16GB RAM", result.Value.Specification);
            Assert.Equal("userA", result.Value.AssignedTo);
            Assert.Equal("admin", result.Value.AssignedBy);
            Assert.Equal("01/01/2024", result.Value.AssignedDate);
            Assert.Equal("Accepted", result.Value.State);
            Assert.Equal("Handle with care", result.Value.Note);
        }

        [Fact]
        public async Task Handle_AssignmentExists_CallsFirstOrDefaultAsyncOnce()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse());

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            repoMock.Verify(
                x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AssignmentWithEmptyOptionalFields_ReturnsEmptyStrings()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock();

            var assignmentId = Guid.NewGuid();
            var response = new Response(
                assignmentId,
                "AS002",
                "Mouse Logitech",
                "",           // Specification → ""
                "userB",
                "admin",
                "15/06/2024",
                "WaitingForAcceptance",
                "");          // Note → ""

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(assignmentId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("", result.Value.Specification);
            Assert.Equal("", result.Value.Note);
        }

        [Fact]
        public async Task Handle_ValidQuery_CancellationTokenIsPassedToRepository()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock();
            using var cts = new CancellationTokenSource();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse());

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            await handler.Handle(query, cts.Token);

            // Assert
            repoMock.Verify(
                x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    cts.Token),
                Times.Once);
        }

        // Assignment not found

        [Fact]
        public async Task Handle_AssignmentNotFound_ReturnsIsError()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Response?)null);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public async Task Handle_AssignmentNotFound_ReturnsCorrectErrorCode()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Response?)null);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal("ViewAdminAssignments.AssignmentNotFoundWithId", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_AssignmentNotFound_ErrorDescriptionContainsAssignmentId()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock();

            var assignmentId = Guid.NewGuid();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Response?)null);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(assignmentId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Contains(assignmentId.ToString(), result.FirstError.Description);
        }

        // Authentication / Authorization

        [Fact]
        public async Task Handle_UserNotAuthenticated_ReturnsUnauthorizedError()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock(isAuthenticated: false);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("ViewAdminAssignments.UnauthorizedUser", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_UserNotAuthenticated_DoesNotCallRepository()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock(isAuthenticated: false);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            repoMock.Verify(
                x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_UserIdIsNull_ReturnsUnidentifiedUserError()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();

            var currentUserMock = new Mock<ICurrentUser>();
            currentUserMock.Setup(x => x.IsAuthenticated).Returns(true);
            currentUserMock.Setup(x => x.UserId).Returns((Guid?)null);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("ViewAdminAssignments.UnidentifiedUser", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_UserIdIsNull_DoesNotCallRepository()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();

            var currentUserMock = new Mock<ICurrentUser>();
            currentUserMock.Setup(x => x.IsAuthenticated).Returns(true);
            currentUserMock.Setup(x => x.UserId).Returns((Guid?)null);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            repoMock.Verify(
                x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // User not found / deleted

        [Fact]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("ViewAdminAssignments.UserNotFound", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_DeletedUser_ReturnsUserNotFoundError()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock();

            var deletedUser = new User { LocationId = Guid.NewGuid(), IsDeleted = true };

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(deletedUser);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("ViewAdminAssignments.UserNotFound", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_UserNotFound_DoesNotCallRepository()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            repoMock.Verify(
                x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // Exception propagation

        [Fact]
        public async Task Handle_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var userManagerMock = CreateUserManagerMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("DB error"));

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query(Guid.NewGuid());

            // Act
            var act = () => handler.Handle(query, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(act);
        }
    }
}
