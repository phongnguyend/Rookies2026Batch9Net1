using Ardalis.Specification;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assignments.GetAll;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.ViewAdminAssignments
{
    public class GetAllHanderTest
    {
        // ── Factories ─────────────────────────────────────────────

        private static Mock<UserManager<User>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private static Mock<IRepository<Assignment, Guid>> CreateRepoMock() =>
            new Mock<IRepository<Assignment, Guid>>();

        private static Mock<ICurrentUser> CreateCurrentUserMock(Guid? userId = null)
        {
            var mock = new Mock<ICurrentUser>();
            mock.Setup(x => x.IsAuthenticated).Returns(true);
            mock.Setup(x => x.UserId).Returns(userId ?? Guid.NewGuid());
            return mock;
        }
        // ── Fixtures ──────────────────────────────────────────────

        private static User CreateUser(Guid? locationId = null) =>
            new User { LocationId = locationId ?? Guid.NewGuid(), IsDeleted = false };

        private static Response CreateResponse(string assetCode = "AS001") =>
            new Response(Guid.NewGuid(), assetCode, "Laptop", "userA", "admin", "01/01/2024", "Accepted", false);

        private static Handler CreateHandler(
            Mock<IRepository<Assignment, Guid>> repo,
            Mock<ICurrentUser> currentUser,
            Mock<UserManager<User>> userManager,
            IValidator<Query>? validator = null) =>
            new Handler(repo.Object, currentUser.Object, userManager.Object, validator ?? new Validator());

        // ── Success cases ─────────────────────────────────────────

        [Fact]
        public async Task Handle_NoFilterApplied_ReturnsPagedListWithAllItems()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            var user = CreateUser();
            var responses = new List<Response>
        {
            CreateResponse("AS001"),
            CreateResponse("AS002"),
        };

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            repoMock
                .Setup(x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);
            repoMock
                .Setup(x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(responses);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(2, result.Value.TotalCount);
            Assert.Equal(2, result.Value.Items.Count);
        }

        [Fact]
        public async Task Handle_NoAssignmentsMatchFilter_ReturnsEmptyPagedList()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            repoMock
                .Setup(x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Response>());

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { SearchTerm = "NonExistentAsset", PageNumber = 1, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(0, result.Value.TotalCount);
            Assert.Empty(result.Value.Items);
        }

        [Fact]
        public async Task Handle_NullPageNumberAndPageSize_UsesDefaultPaginationValues()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            repoMock
                .Setup(x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Response>());

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { PageNumber = null, PageSize = null };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.PageNumber);
            Assert.Equal(10, result.Value.PageSize);
        }

        [Fact]
        public async Task Handle_ValidQuery_ReturnsCorrectPaginationMetadata()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            var items = Enumerable.Range(1, 5).Select(i => CreateResponse($"AS00{i}")).ToList();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(50);
            repoMock
                .Setup(x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(items);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { PageNumber = 3, PageSize = 5 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(3, result.Value.PageNumber);
            Assert.Equal(5, result.Value.PageSize);
            Assert.Equal(50, result.Value.TotalCount);
        }

        // ── Repository call verification ──────────────────────────

        [Fact]
        public async Task Handle_ValidQuery_CallsCountAsyncExactlyOnce()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            repoMock
                .Setup(x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Response>());

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { PageNumber = 1, PageSize = 10 };

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            repoMock.Verify(
                x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ValidQuery_CallsListAsyncExactlyOnce()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            repoMock
                .Setup(x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Response>());

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { PageNumber = 1, PageSize = 10 };

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            repoMock.Verify(
                x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // ── Validation failure ─────────────────────────────────────

        [Fact]
        public async Task Handle_InvalidState_ThrowsValidationException()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { State = new[] { "InvalidState" } };

            // Act
            var act = () => handler.Handle(query, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(act);
        }

        [Fact]
        public async Task Handle_InvalidState_DoesNotCallRepository()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { State = new[] { "InvalidState" } };

            // Act
            try { await handler.Handle(query, CancellationToken.None); } catch { /* expected */ }

            // Assert
            repoMock.Verify(
                x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()),
                Times.Never);
            repoMock.Verify(
                x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ── User not found / deleted ───────────────────────────────

        [Fact]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UserNotFound, result.FirstError);
        }

        [Fact]
        public async Task Handle_DeletedUser_ReturnsUserNotFoundError()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            var deletedUser = new User { LocationId = Guid.NewGuid(), IsDeleted = true };

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(deletedUser);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UserNotFound, result.FirstError);
        }

        [Fact]
        public async Task Handle_UserNotFound_DoesNotCallRepository()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { PageNumber = 1, PageSize = 10 };

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            repoMock.Verify(
                x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()),
                Times.Never);
            repoMock.Verify(
                x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ── CancellationToken propagation ─────────────────────────

        [Fact]
        public async Task Handle_CancellationToken_IsPassedToCountAsync()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();
            using var cts = new CancellationTokenSource();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            repoMock
                .Setup(x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Response>());

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { PageNumber = 1, PageSize = 10 };

            // Act
            await handler.Handle(query, cts.Token);

            // Assert
            repoMock.Verify(
                x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), cts.Token),
                Times.Once);
        }

        [Fact]
        public async Task Handle_CancellationToken_IsPassedToListAsync()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();
            using var cts = new CancellationTokenSource();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            repoMock
                .Setup(x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Response>());

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { PageNumber = 1, PageSize = 10 };

            // Act
            await handler.Handle(query, cts.Token);

            // Assert
            repoMock.Verify(
                x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), cts.Token),
                Times.Once);
        }

        // ── Exception propagation ──────────────────────────────────

        [Fact]
        public async Task Handle_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());
            repoMock
                .Setup(x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("DB error"));

            var handler = CreateHandler(repoMock, currentUserMock, userManagerMock);
            var query = new Query { PageNumber = 1, PageSize = 10 };

            // Act
            var act = () => handler.Handle(query, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(act);
        }
    }
}
