using Ardalis.Specification;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.ReturnRequests.UserCreateReturnRequest;
using NashAssetManagement.Domain.Entities.Core;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.ReturnRequests.UserCreateReturnRequest
{
    public class HandlerTests
    {
        private readonly Mock<IRepository<Assignment, Guid>> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ILogger<Handler>> _mockLogger;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<IValidator<Request>> _mockValidator;
        private readonly Mock<IDateTimeProvider> _mockDateTime;
        private readonly Handler _handler;

        public HandlerTests()
        {
            _mockRepo = new Mock<IRepository<Assignment, Guid>>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<Handler>>();
            _mockUser = new Mock<ICurrentUser>();
            _mockValidator = new Mock<IValidator<Request>>();
            _mockDateTime = new Mock<IDateTimeProvider>();

            _handler = new Handler(_mockRepo.Object,
                _mockUow.Object,
                _mockLogger.Object,
                _mockUser.Object,
                _mockValidator.Object,
                _mockDateTime.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());
            var validationFailures = new List<ValidationFailure> { new("AssignmentId", "Invalid Format") };

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var act = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(act);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorizedUser_WhenUserNotAuthenticated()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(false);

            // Act
            ErrorOr<Created> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnauthorizedUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnidentifiedUser_WhenUserIdIsNull()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns((Guid?)null);

            // Act
            ErrorOr<Created> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnidentifiedUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFoundError_WhenAssignmentNotFound()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(Guid.NewGuid());

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Assignment?)null);

            // Act
            ErrorOr<Created> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentNotFoundWithId(request.AssignmentId!).Description, result.FirstError.Description);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotAssignedError_WhenAssignmentNotAssignedToCurrentUser()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var currentUserId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid();

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(currentUserId);

            var assignment = new Assignment
            {
                Id = assignmentId,
                AssignedToUserId = differentUserId,
                State = Domain.Enums.AssignmentState.Accepted,
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Created> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentNotAssignedToUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnHasWaitingRequestError_WhenCannotCreateRequestRulesViolated()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var userId = Guid.NewGuid();

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);

            var assignment = new Assignment
            {
                Id = assignmentId,
                AssignedToUserId = userId,
                IsReturning = false,
                State = Domain.Enums.AssignmentState.Accepted,
                ReturnRequests = [new ReturnRequest { State = Domain.Enums.ReturnRequestState.WaitingForReturning }]
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Created> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentHasWaitingReturnRequest, result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldLogErrorAndReturnUnexpectedError_WhenDatabaseSaveFails()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var userId = Guid.NewGuid();

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);
            _mockDateTime.Setup(d => d.UtcNow).Returns(DateTime.UtcNow);

            var assignment = new Assignment
            {
                Id = assignmentId,
                AssignedToUserId = userId,
                IsReturning = false,
                ReturnRequests = [],
                State = Domain.Enums.AssignmentState.Accepted,
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            _mockUow
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection drop."));

            // Act
            ErrorOr<Created> result = await _handler.Handle(request, CancellationToken.None);

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
        public async Task Handle_ShouldUpdateAssignmentStateAndReturnCreated_WhenHappyPath()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var userId = Guid.NewGuid();
            var mockTime = DateTime.UtcNow;

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);
            _mockDateTime.Setup(d => d.UtcNow).Returns(mockTime);

            var assignment = new Assignment
            {
                Id = assignmentId,
                AssignedToUserId = userId,
                IsReturning = false,
                ReturnRequests = new List<ReturnRequest>(),
                State = Domain.Enums.AssignmentState.Accepted,
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Created> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Created, result.Value);
            Assert.True(assignment.IsReturning);
            Assert.Single(assignment.ReturnRequests);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(Domain.Enums.AssignmentState.Declined)]
        [InlineData(Domain.Enums.AssignmentState.WaitingForAcceptance)]
        [InlineData(Domain.Enums.AssignmentState.Returned)]
        public async Task Handle_ShouldReturnInvalidAssignmentStateError_WhenAssignmentStateIsNotAccepted(Domain.Enums.AssignmentState state)
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var userId = Guid.NewGuid();

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);

            var assignment = new Assignment
            {
                Id = assignmentId,
                AssignedToUserId = userId,
                State = state,
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Created> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidAssignmentState, result.FirstError);
        }
    }
}
