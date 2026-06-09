using Ardalis.Specification;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.ReturnRequests.ViewList;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.ReturnRequests.ViewList
{
    public class HandlerTests
    {
        private static readonly Guid CurrentUserId = Guid.Parse("42af4a73-f42e-40bf-9075-6c9b559df450");
        private const string LocationId = "a3b7ef5a-bce7-401f-bbe3-94c2f7bf0b94";

        private readonly Mock<IRepository<ReturnRequest, Guid>> _mockRepo;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<IValidator<Request>> _mockValidator;
        private readonly Handler _handler;

        public HandlerTests()
        {
            _mockRepo = new Mock<IRepository<ReturnRequest, Guid>>();
            _mockUser = new Mock<ICurrentUser>();
            _mockValidator = new Mock<IValidator<Request>>();

            _handler = new Handler(
                _mockRepo.Object,
                _mockUser.Object,
                _mockValidator.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var request = new Request(null, null, null, null, null, 10, 0);
            var errors = new List<ValidationFailure>
            {
                new(nameof(Request.PageNumber), "Page number must be greater than 0.")
            };

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act
            var act = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(act);
            _mockRepo.Verify(
                r => r.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _mockRepo.Verify(
                r => r.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorizedError_WhenCannotGetUserId()
        {
            // Arrange
            var request = new Request(null, null, null, null, null, 10, 1);

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.UserId).Returns((Guid?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.Unauthorized, result.FirstError);
            _mockRepo.Verify(
                r => r.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _mockRepo.Verify(
                r => r.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedList_WhenRequestIsValidAndUserIsAuthorized()
        {
            // Arrange
            var request = new Request(
                SearchTerm: "laptop",
                States: [ReturnRequestState.Completed],
                ReturnedDate: "2026-05-26",
                SortBy: " assetCode ",
                SortDesc: false,
                PageSize: 2,
                PageNumber: 2);
            var responses = CreateResponses();
            const int expectedTotal = 5;

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.UserId).Returns(CurrentUserId);
            _mockUser.Setup(u => u.LocationId).Returns(LocationId);
            _mockRepo
                .Setup(r => r.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTotal);
            _mockRepo
                .Setup(r => r.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(responses);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedTotal, result.Value.TotalCount);
            Assert.Equal(2, result.Value.PageNumber);
            Assert.Equal(2, result.Value.PageSize);
            Assert.Equal(responses, result.Value.Items);
            _mockRepo.Verify(
                r => r.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _mockRepo.Verify(
                r => r.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldExcludeCancelledReturnRequests_WhenViewingList()
        {
            // Arrange
            var request = new Request(null, null, null, null, null, 10, 1);
            ISpecification<ReturnRequest>? countSpec = null;
            ISpecification<ReturnRequest, Response>? listSpec = null;

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.UserId).Returns(CurrentUserId);
            _mockUser.Setup(u => u.LocationId).Returns(LocationId);
            _mockRepo
                .Setup(r => r.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()))
                .Callback<ISpecification<ReturnRequest>, CancellationToken>((spec, _) => countSpec = spec)
                .ReturnsAsync(0);
            _mockRepo
                .Setup(r => r.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()))
                .Callback<ISpecification<ReturnRequest, Response>, CancellationToken>((spec, _) => listSpec = spec)
                .ReturnsAsync([]);

            var waitingRequest = CreateReturnRequest(ReturnRequestState.WaitingForReturning);
            var completedRequest = CreateReturnRequest(ReturnRequestState.Completed);
            var cancelledRequest = CreateReturnRequest(ReturnRequestState.Cancelled);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.NotNull(countSpec);
            Assert.NotNull(listSpec);
            Assert.True(MatchesFilters(countSpec, waitingRequest));
            Assert.True(MatchesFilters(countSpec, completedRequest));
            Assert.False(MatchesFilters(countSpec, cancelledRequest));
            Assert.True(MatchesFilters(listSpec, waitingRequest));
            Assert.True(MatchesFilters(listSpec, completedRequest));
            Assert.False(MatchesFilters(listSpec, cancelledRequest));
        }

        [Fact]
        public async Task Handle_ShouldUseDefaultPaging_WhenPageNumberAndPageSizeAreNull()
        {
            // Arrange
            var request = new Request(null, null, null, null, null, null, null);
            var responses = CreateResponses();
            const int expectedTotal = 2;

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.UserId).Returns(CurrentUserId);
            _mockUser.Setup(u => u.LocationId).Returns(LocationId);
            _mockRepo
                .Setup(r => r.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTotal);
            _mockRepo
                .Setup(r => r.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(responses);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedTotal, result.Value.TotalCount);
            Assert.Equal(1, result.Value.PageNumber);
            Assert.Equal(20, result.Value.PageSize);
            Assert.Equal(responses, result.Value.Items);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToRepository_WhenRequestIsValid()
        {
            // Arrange
            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var request = new Request(null, null, null, null, null, 10, 1);

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.UserId).Returns(CurrentUserId);
            _mockUser.Setup(u => u.LocationId).Returns(LocationId);
            _mockRepo
                .Setup(r => r.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            _mockRepo
                .Setup(r => r.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.False(result.IsError);
            _mockRepo.Verify(
                r => r.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), cancellationToken),
                Times.Once);
            _mockRepo.Verify(
                r => r.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldValidateCleanedRequest_WhenSearchTermAndSortByHaveExtraSpaces()
        {
            // Arrange
            var request = new Request(" laptop     pro ", null, null, " assetCode ", null, null, null);

            _mockValidator
                .Setup(v => v.ValidateAsync(
                    It.Is<Request>(r =>
                        r.SearchTerm == "laptop pro"
                        && r.SortBy == "assetCode"
                        && r.PageNumber == 1
                        && r.PageSize == 20),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.UserId).Returns(CurrentUserId);
            _mockUser.Setup(u => u.LocationId).Returns(LocationId);
            _mockRepo
                .Setup(r => r.CountAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            _mockRepo
                .Setup(r => r.ListAsync(It.IsAny<ISpecification<ReturnRequest, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            _mockValidator.Verify(
                v => v.ValidateAsync(
                    It.Is<Request>(r =>
                        r.SearchTerm == "laptop pro"
                        && r.SortBy == "assetCode"
                        && r.PageNumber == 1
                        && r.PageSize == 20),
                    It.IsAny<CancellationToken>()),
                Times.Once);
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
                    new DateTime(2026, 5, 20),
                    "admin",
                    new DateTime(2026, 5, 26),
                    ReturnRequestState.Completed.ToString()),
                new(
                    Guid.Parse("d7f151a1-88f3-4f77-9b75-7d85086f7d02"),
                    "MO000001",
                    "Monitor",
                    "huongtt",
                    new DateTime(2026, 5, 21),
                    null,
                    null,
                    ReturnRequestState.WaitingForReturning.ToString())
            ];
        }

        private static ReturnRequest CreateReturnRequest(ReturnRequestState state)
        {
            return new ReturnRequest
            {
                State = state,
                Assignment = new Assignment
                {
                    Asset = new Asset
                    {
                        LocationId = Guid.Parse(LocationId),
                        AssetCode = "LA000001",
                        Name = "Laptop"
                    }
                }
            };
        }

        private static bool MatchesFilters(ISpecification<ReturnRequest> spec, ReturnRequest request)
        {
            return spec.WhereExpressions.All(expression => expression.Filter.Compile()(request));
        }
    }
}
