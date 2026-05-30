using Ardalis.Specification;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Assignments.UserAcceptingAssignment;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.UserAcceptingAssignment
{
    public class HandlerTests
    {
        readonly Mock<IRepository<Assignment, Guid>> _mockRepo;
        readonly Mock<IUnitOfWork> _mockUow;
        readonly Mock<ICurrentUser> _mockUser;
        readonly Mock<IValidator<Request>> _mockValidator;
        readonly Mock<IDateTimeProvider> _mockDateTime;
        readonly Mock<ILogger<Handler>> _mockLogger;
        readonly Handler _handler;

        public HandlerTests()
        {
            _mockRepo = new Mock<IRepository<Assignment, Guid>>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockUser = new Mock<ICurrentUser>();
            _mockValidator = new Mock<IValidator<Request>>();
            _mockDateTime = new Mock<IDateTimeProvider>();
            _mockLogger = new Mock<ILogger<Handler>>();

            _handler = new Handler(
                _mockRepo.Object,
                _mockUow.Object,
                _mockUser.Object,
                _mockValidator.Object,
                _mockDateTime.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());
            var errors = new List<ValidationFailure> { new("AssignmentId", "Invalid Format") };

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenUserNotAuthenticated_ShouldReturnUnauthorizedUser()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());
            SetupValidationSuccess(request);

            _mockUser.Setup(u => u.IsAuthenticated).Returns(false);

            // Act
            ErrorOr<Updated> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnauthorizedUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenUserIdIsNull_ShouldReturnUnidentifiedUser()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());
            SetupValidationSuccess(request);

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns((Guid?)null);

            // Act
            ErrorOr<Updated> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnidentifiedUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenAssignmentNotFound_ShouldReturnNotFoundError()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());
            SetupValidationSuccess(request);
            SetupUserAuthenticated(Guid.NewGuid());

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Assignment?)null);

            // Act
            ErrorOr<Updated> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentNotFoundWithId(request.AssignmentId!).Description, result.FirstError.Description);
        }

        [Fact]
        public async Task Handle_WhenAssignmentStateIsInvalid_ShouldReturnInvalidAssignmentStateError()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            SetupValidationSuccess(request);
            SetupUserAuthenticated(Guid.NewGuid());

            var assignment = new Assignment
            {
                State = AssignmentState.Accepted // Not WaitingForAcceptance
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Updated> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidAssignmentState, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenAssignmentNotAssignedToCurrentUser_ShouldReturnNotAssignedError()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var currentUserId = Guid.NewGuid();
            SetupValidationSuccess(request);
            SetupUserAuthenticated(currentUserId);

            var assignment = new Assignment
            {
                State = AssignmentState.WaitingForAcceptance,
                AssignedToUserId = Guid.NewGuid() // Belongs to someone else
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Updated> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentNotAssignedToUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenAssignmentAssetIsNull_ShouldReturnAssetNotFoundError()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var userId = Guid.NewGuid();
            SetupValidationSuccess(request);
            SetupUserAuthenticated(userId);

            var assignment = new Assignment
            {
                State = AssignmentState.WaitingForAcceptance,
                AssignedToUserId = userId,
                Asset = null // Missing asset relation
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Updated> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssetOfAssignmentNotFound(request.AssignmentId!).Description, result.FirstError.Description);
        }

        [Fact]
        public async Task Handle_WhenAssignmentAssetStateIsInvalid_ShouldReturnInvalidAssignmentAssetStateError()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var userId = Guid.NewGuid();
            SetupValidationSuccess(request);
            SetupUserAuthenticated(userId);

            var assignment = new Assignment
            {
                State = AssignmentState.WaitingForAcceptance,
                AssignedToUserId = userId,
                Asset = new Asset { State = AssetState.NotAvailable }
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Updated> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidAssignmentAssetState, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenDatabaseSaveThrowsException_ShouldLogErrorAndReturnUnexpectedError()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var userId = Guid.NewGuid();
            SetupValidationSuccess(request);
            SetupUserAuthenticated(userId);
            _mockDateTime.Setup(d => d.UtcNow).Returns(DateTime.UtcNow);

            var assignment = new Assignment
            {
                State = AssignmentState.WaitingForAcceptance,
                AssignedToUserId = userId,
                Asset = new Asset { State = AssetState.Assigned }
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            _mockUow
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database context update failed"));

            // Act
            ErrorOr<Updated> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnexpectedErrorOccurred, result.FirstError);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenHappyPath_ShouldInvokeAcceptDomainMethodAndReturnUpdated()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var userId = Guid.NewGuid();
            var fakeNow = DateTime.UtcNow;

            SetupValidationSuccess(request);
            SetupUserAuthenticated(userId);
            _mockDateTime.Setup(d => d.UtcNow).Returns(fakeNow);

            var assignment = new Assignment
            {
                State = AssignmentState.WaitingForAcceptance,
                AssignedToUserId = userId,
                Asset = new Asset { State = AssetState.Assigned }
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Updated> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify state mutated correctly through domain logic
            Assert.Equal(AssignmentState.Accepted, assignment.State);
            Assert.Equal(fakeNow, assignment.UpdatedAtUtc);

            // Verify database persist was called
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private void SetupValidationSuccess(Request request)
        {
            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }

        private void SetupUserAuthenticated(Guid userId)
        {
            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);
        }
    }
}
