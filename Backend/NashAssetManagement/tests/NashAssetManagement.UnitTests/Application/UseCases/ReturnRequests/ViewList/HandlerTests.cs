using System.Reflection;
using Ardalis.Specification;
using ErrorOr;
using FluentValidation;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.ReturnRequests.ViewList;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.ReturnRequests.ViewList
{
    public class HandlerTests
    {
        private static readonly Guid CurrentUserId = Guid.Parse("42af4a73-f42e-40bf-9075-6c9b559df450");
        private static readonly Guid LocationId = Guid.Parse("a3b7ef5a-bce7-401f-bbe3-94c2f7bf0b94");

        [Fact]
        public async Task Handle_CurrentUserIdIsNull_ShouldReturnUnauthorized()
        {
            // Arrange
            var repositoryMock = CreateRepositoryMock();
            var currentUserMock = CreateCurrentUserMock(null, LocationId.ToString());
            var request = new Request(null, null, null, null, null, 10, 1);

            // Act
            var result = await HandleAsync(repositoryMock, currentUserMock, request);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("ReturnRequests.ViewList.Unauthorized", result.FirstError.Code);

            repositoryMock.Verify(
                x => x.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()),
                Times.Never);

            repositoryMock.Verify(
                x => x.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidPageNumber_ShouldThrowValidationException()
        {
            // Arrange
            var repositoryMock = CreateRepositoryMock();
            var currentUserMock = CreateCurrentUserMock(CurrentUserId, LocationId.ToString());
            var request = new Request(null, null, null, null, null, 10, 0);

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => HandleAsync(repositoryMock, currentUserMock, request));

            // Assert
            Assert.Contains(exception.Errors,
                x => x.PropertyName == nameof(Request.PageNumber)
                     && x.ErrorMessage == "Page number must be greater than 0.");

            repositoryMock.Verify(
                x => x.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()),
                Times.Never);

            repositoryMock.Verify(
                x => x.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidState_ShouldThrowValidationException()
        {
            // Arrange
            var repositoryMock = CreateRepositoryMock();
            var currentUserMock = CreateCurrentUserMock(CurrentUserId, LocationId.ToString());
            var request = new Request(null, ["InvalidState"], null, null, null, 10, 1);

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => HandleAsync(repositoryMock, currentUserMock, request));

            // Assert
            Assert.Contains(exception.Errors,
                x => x.PropertyName.StartsWith(nameof(Request.States))
                     && x.ErrorMessage == "Invalid state value: InvalidState.");

            repositoryMock.Verify(
                x => x.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()),
                Times.Never);

            repositoryMock.Verify(
                x => x.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnPagedReturnRequests()
        {
            // Arrange
            var items = CreateResponses();
            var repositoryMock = CreateRepositoryMock(totalItems: 5, items);
            var currentUserMock = CreateCurrentUserMock(CurrentUserId, LocationId.ToString());
            var request = new Request(
                SearchTerm: "laptop",
                States: [ReturnRequestState.Completed.ToString()],
                ReturnedDate: "2026-05-26",
                SortBy: " assetCode ",
                SortDesc: false,
                PageSize: 2,
                PageNumber: 2);

            // Act
            var result = await HandleAsync(repositoryMock, currentUserMock, request);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(5, result.Value.TotalCount);
            Assert.Equal(2, result.Value.PageNumber);
            Assert.Equal(2, result.Value.PageSize);
            Assert.Equal(items, result.Value.Items);

            repositoryMock.Verify(
                x => x.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()),
                Times.Once);

            repositoryMock.Verify(
                x => x.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_NullPageNumberAndPageSize_ShouldUseDefaults()
        {
            // Arrange
            var items = CreateResponses();
            var repositoryMock = CreateRepositoryMock(totalItems: 2, items);
            var currentUserMock = CreateCurrentUserMock(CurrentUserId, LocationId.ToString());
            var request = new Request(null, null, null, null, null, null, null);

            // Act
            var result = await HandleAsync(repositoryMock, currentUserMock, request);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(1, result.Value.PageNumber);
            Assert.Equal(20, result.Value.PageSize);
            Assert.Equal(2, result.Value.TotalCount);
            Assert.Equal(items, result.Value.Items);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var repositoryMock = CreateRepositoryMock(totalItems: 0, []);
            var currentUserMock = CreateCurrentUserMock(CurrentUserId, LocationId.ToString());
            var request = new Request(null, null, null, null, null, 10, 1);

            // Act
            var result = await HandleAsync(repositoryMock, currentUserMock, request, cancellationToken);

            // Assert
            Assert.False(result.IsError);

            repositoryMock.Verify(
                x => x.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), cancellationToken),
                Times.Once);

            repositoryMock.Verify(
                x => x.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), cancellationToken),
                Times.Once);
        }

        private static async Task<ErrorOr<PagedList<Response>>> HandleAsync(
            Mock<IRepository<ReturnRequest, Guid>> repositoryMock,
            Mock<ICurrentUser> currentUserMock,
            Request request,
            CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(Request).Assembly.GetType(
                "NashAssetManagement.Application.UseCases.ReturnRequests.ViewList.Handler",
                throwOnError: true)!;

            var handler = Activator.CreateInstance(
                handlerType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                args: [repositoryMock.Object, currentUserMock.Object, new Validators()],
                culture: null)!;

            var handleMethod = handlerType.GetMethod(
                "Handle",
                BindingFlags.Instance | BindingFlags.Public)!;

            var task = (Task<ErrorOr<PagedList<Response>>>)handleMethod.Invoke(
                handler,
                [request, cancellationToken])!;

            return await task;
        }

        private static Mock<IRepository<ReturnRequest, Guid>> CreateRepositoryMock(
            int totalItems = 0,
            List<Response>? items = null)
        {
            var repositoryMock = new Mock<IRepository<ReturnRequest, Guid>>();

            repositoryMock
                .Setup(x => x.CountAsync(
                    It.IsAny<ISpecification<ReturnRequest>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(totalItems);

            repositoryMock
                .Setup(x => x.ListAsync(
                    It.IsAny<ISpecification<ReturnRequest, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(items ?? []);

            return repositoryMock;
        }

        private static Mock<ICurrentUser> CreateCurrentUserMock(Guid? userId, string? locationId)
        {
            var currentUserMock = new Mock<ICurrentUser>();

            currentUserMock
                .Setup(x => x.UserId)
                .Returns(userId);

            currentUserMock
                .Setup(x => x.LocationId)
                .Returns(locationId);

            return currentUserMock;
        }

        private static List<Response> CreateResponses()
        {
            return
            [
                new(
                    Guid.Parse("d7f151a1-88f3-4f77-9b75-7d85086f7d01"),
                    "LA000001",
                    "Laptop",
                    "binhnv",
                    "2026-05-20",
                    "admin",
                    "2026-05-26",
                    ReturnRequestState.Completed.ToString()),
                new(
                    Guid.Parse("d7f151a1-88f3-4f77-9b75-7d85086f7d02"),
                    "MO000001",
                    "Monitor",
                    "huongtt",
                    "2026-05-21",
                    null,
                    null,
                    ReturnRequestState.WaitingForReturning.ToString())
            ];
        }
    }
}
