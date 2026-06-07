using Ardalis.Specification;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.ReturnRequests.AdminCancelReturnRequest;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.ReturnRequests.AdminCancelReturnRequest
{
    public class HandlerTests
    {
        readonly Mock<IRepository<ReturnRequest, Guid>> _mockRepo;
        readonly Mock<IUnitOfWork> _mockUow;
        readonly Mock<ICurrentUser> _mockUser;
        readonly Mock<ILogger<Handler>> _mockLogger;
        readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
        readonly Mock<IValidator<Request>> _mockValidator;
        readonly Handler _handler;
        readonly DateTime _fakeNow;

        public HandlerTests()
        {
            _mockRepo = new Mock<IRepository<ReturnRequest, Guid>>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockUser = new Mock<ICurrentUser>();
            _mockLogger = new Mock<ILogger<Handler>>();
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _mockValidator = new Mock<IValidator<Request>>();

            _fakeNow = new DateTime(2026, 6, 6, 12, 0, 0, DateTimeKind.Utc);
            _mockDateTimeProvider.Setup(d => d.UtcNow).Returns(_fakeNow);

            _handler = new Handler(
                _mockRepo.Object,
                _mockUow.Object,
                _mockUser.Object,
                _mockLogger.Object,
                _mockDateTimeProvider.Object,
                _mockValidator.Object
            );
        }

        #region 1. Request, Authentication & Location Guard Tests

        [Fact]
        public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
        {
            // Arrange
            var request = new Request(ReturnRequestId: "invalid-id");
            var errors = new List<ValidationFailure> { new("ReturnRequestId", "ID format is invalid") };
            _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult(errors));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenUserNotAuthenticated_ShouldReturnUnauthorizedUser()
        {
            // Arrange
            var request = CreateDefaultRequest();
            SetupValidationSuccess(request);
            _mockUser.Setup(u => u.IsAuthenticated).Returns(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnauthorizedUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenUserIsNotAdmin_ShouldReturnNotAdminUser()
        {
            // Arrange
            var request = CreateDefaultRequest();
            SetupValidationSuccess(request);
            SetupUserAuthenticated();
            _mockUser.Setup(u => u.Roles).Returns(new List<string> { ApplicationRole.Staff });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.NotAdminUser, result.FirstError);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("not-a-valid-guid")]
        public async Task Handle_WhenAdminLocationIdIsMalformed_ShouldReturnLocationNotFound(string? invalidLocationId)
        {
            // Arrange
            var request = CreateDefaultRequest();
            SetupValidationSuccess(request);
            SetupUserAuthenticated();
            SetupAdminRole();
            _mockUser.Setup(u => u.LocationId).Returns(invalidLocationId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.LocationNotFound, result.FirstError);
        }

        #endregion

        #region 2. Return Request Domain Rules Tests

        [Fact]
        public async Task Handle_WhenReturnRequestNotFound_ShouldReturnRequestNotFoundError()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = CreateDefaultRequest(requestId.ToString());
            SetupValidationSuccess(request);
            SetupAdminContext();

            _mockRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReturnRequest?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.RequestNotFound(requestId.ToString()).Description, result.FirstError.Description);
        }

        [Fact]
        public async Task Handle_WhenReturnRequestLacksAssignmentRelation_ShouldReturnAssignmentNotFound()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = CreateDefaultRequest(requestId.ToString());
            SetupValidationSuccess(request);
            SetupAdminContext();

            // Concrete instance with a missing (null) Assignment reference navigation property
            var returnRequest = new ReturnRequest
            {
                Id = requestId,
                State = ReturnRequestState.WaitingForReturning,
                Assignment = null
            };

            _mockRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnRequest);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentNotFound, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenReturnRequestStateCannotBeCancelled_ShouldReturnInvalidRequestState()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = CreateDefaultRequest(requestId.ToString());
            SetupValidationSuccess(request);
            SetupAdminContext();

            // Concrete instance set to a state where CanCancel() evaluates to false (e.g., Completed)
            var returnRequest = new ReturnRequest
            {
                Id = requestId,
                State = ReturnRequestState.Completed,
                Assignment = new Assignment { Id = Guid.NewGuid() }
            };

            _mockRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnRequest);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidRequestState, result.FirstError);
        }

        #endregion

        #region 3. Persistence Faults & Happy Path State Checks

        [Fact]
        public async Task Handle_WhenDatabaseSaveFails_ShouldLogErrorAndReturnUnexpectedError()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = CreateDefaultRequest(requestId.ToString());
            SetupValidationSuccess(request);
            SetupAdminContext();

            var returnRequest = new ReturnRequest
            {
                Id = requestId,
                State = ReturnRequestState.WaitingForReturning,
                Assignment = new Assignment { Id = Guid.NewGuid(), IsReturning = true }
            };

            _mockRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnRequest);

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection failure during transaction block."));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnexpectedErrorOccurred, result.FirstError);

            // Verify the infrastructure catch block logged the error appropriately
            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenHappyPath_ShouldCancelReturnRequestReleaseAssignmentAndReturnUpdated()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = CreateDefaultRequest(requestId.ToString());
            SetupValidationSuccess(request);
            SetupAdminContext();

            // Build fully linked concrete objects using standard setup values
            var associatedAssignment = new Assignment
            {
                Id = Guid.NewGuid(),
                IsReturning = true,
                UpdatedAtUtc = _fakeNow.AddDays(-5) // Old state timestamp
            };

            var returnRequest = new ReturnRequest
            {
                Id = requestId,
                State = ReturnRequestState.WaitingForReturning,
                UpdatedAtUtc = _fakeNow.AddDays(-5),
                Assignment = associatedAssignment
            };

            _mockRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<ReturnRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnRequest);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // 1. Verify ReturnRequest state mutations
            Assert.Equal(ReturnRequestState.Cancelled, returnRequest.State);
            Assert.Equal(_fakeNow, returnRequest.UpdatedAtUtc);

            // 2. Verify cascading Assignment entity adjustments
            Assert.False(returnRequest.Assignment.IsReturning);
            Assert.Equal(_fakeNow, returnRequest.Assignment.UpdatedAtUtc);

            // 3. Verify unit of work commit occurred cleanly
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region ⚙️ Private Setup Matrix Helpers

        private Request CreateDefaultRequest(string? requestId = null)
        {
            return new Request(ReturnRequestId: requestId ?? Guid.NewGuid().ToString());
        }

        private void SetupValidationSuccess(Request request) =>
            _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        private void SetupUserAuthenticated() =>
            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);

        private void SetupAdminRole() =>
            _mockUser.Setup(u => u.Roles).Returns(new List<string> { ApplicationRole.Admin });

        private void SetupAdminContext()
        {
            SetupUserAuthenticated();
            SetupAdminRole();
            _mockUser.Setup(u => u.LocationId).Returns(Guid.NewGuid().ToString());
        }

        #endregion
    }
}
