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

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments
{
    public class GetAllHanderTest
    {
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
            mock.Setup(x => x.UserId).Returns(userId ?? Guid.NewGuid());
            return mock;
        }

        [Fact]
        public async Task Handle_Should_Return_PagedList_When_No_Filter_Applied()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();
            var validator = new Validator();

            var locationId = Guid.NewGuid();
            var user = new User { LocationId = locationId };

            var responses = new List<Response>
            {
                new Response(Guid.NewGuid(), "AS001", "Laptop", "userA", "admin", "01/01/2024", "Accepted"),
                new Response(Guid.NewGuid(), "AS002", "Mouse",  "userB", "admin", "02/01/2024", "Waiting")
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

            var handler = new Handler(
                repoMock.Object,
                currentUserMock.Object,
                userManagerMock.Object,
                validator);

            var query = new Query { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(2, result.Value.TotalCount);
            Assert.Equal(2, result.Value.Items.Count);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_PagedList_When_No_Assignments_Match()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();
            var validator = new Validator();

            var user = new User { LocationId = Guid.NewGuid() };

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            repoMock
                .Setup(x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            repoMock
                .Setup(x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Response>());

            var handler = new Handler(
                repoMock.Object,
                currentUserMock.Object,
                userManagerMock.Object,
                validator);

            var query = new Query { SearchTerm = "NonExistentAsset", PageNumber = 1, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(0, result.Value.TotalCount);
            Assert.Empty(result.Value.Items);
        }

        [Fact]
        public async Task Handle_Should_Use_Default_Paging_When_PageNumber_And_PageSize_Are_Null()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();
            var validator = new Validator();

            var user = new User { LocationId = Guid.NewGuid() };

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            repoMock
                .Setup(x => x.CountAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            repoMock
                .Setup(x => x.ListAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Response>());

            var handler = new Handler(
                repoMock.Object,
                currentUserMock.Object,
                userManagerMock.Object,
                validator);

            // PageNumber and PageSize are null → handler defaults to 1 and 10
            var query = new Query { PageNumber = null, PageSize = null };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.PageNumber);
            Assert.Equal(10, result.Value.PageSize);
        }

        [Fact]
        public async Task Handle_Should_Throw_ValidationException_When_Query_Is_Invalid()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var repoMock = CreateRepoMock();
            var currentUserMock = CreateCurrentUserMock();
            var validator = new Validator();

            var handler = new Handler(
                repoMock.Object,
                currentUserMock.Object,
                userManagerMock.Object,
                validator);

            // "InvalidState" is not a valid AssignmentState enum value
            var query = new Query { State = new[] { "InvalidState" } };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () => handler.Handle(query, CancellationToken.None));
        }
    }
}
