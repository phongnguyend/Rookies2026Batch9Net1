using Ardalis.Specification;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Assignments.DeleteAssignment;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.DeleteAssignment
{
    public class HandlerTests
    {
        private readonly Mock<IRepository<Assignment, Guid>> _repoMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
        private readonly Mock<IValidator<Request>> _validatorMock = new();
        private readonly Mock<ILogger<Handler>> _loggerMock = new();
        private readonly Handler _handler;

        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _locationId = Guid.NewGuid();
        private readonly DateTime _utcNow = DateTime.UtcNow;

        public HandlerTests()
        {
            _dateTimeProviderMock
                .Setup(x => x.UtcNow)
                .Returns(_utcNow);

            _handler = new Handler(
                _repoMock.Object,
                _unitOfWorkMock.Object,
                _currentUserMock.Object,
                _dateTimeProviderMock.Object,
                _validatorMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
        {
            // Arrange
            var request = CreateRequest();
            var errors = new List<ValidationFailure>
            {
                new("AssignmentId", "Assignment Id must be a valid Guid/uuid.")
            };

            _validatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act
            Func<Task> act = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(act);

            _repoMock.Verify(
                x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserIsNotAuthenticated_ShouldReturnUnauthorizedUser()
        {
            // Arrange
            var request = CreateRequest();
            SetupValidValidation(request);

            _currentUserMock
                .Setup(x => x.IsAuthenticated)
                .Returns(false);

            // Act
            ErrorOr<Deleted> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnauthorizedUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenCurrentUserIdIsNull_ShouldReturnUnidentifiedUser()
        {
            // Arrange
            var request = CreateRequest();
            SetupValidValidation(request);

            _currentUserMock
                .Setup(x => x.IsAuthenticated)
                .Returns(true);

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns((Guid?)null);

            // Act
            ErrorOr<Deleted> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnidentifiedUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenAssignmentNotFound_ShouldReturnAssignmentNotFound()
        {
            // Arrange
            var request = CreateRequest();
            SetupValidValidation(request);
            SetupAuthenticatedUser();

            _repoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Assignment?)null);

            // Act
            ErrorOr<Deleted> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentNotFound, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenAssignmentStateIsNotWaitingForAcceptance_ShouldReturnInvalidAssignmentState()
        {
            // Arrange
            var request = CreateRequest();
            SetupValidValidation(request);
            SetupAuthenticatedUser();

            var assignment = BuildAssignment(state: AssignmentState.Accepted);

            _repoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Deleted> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidAssignmentState, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenAssignmentIsAlreadyDeleted_ShouldReturnAssignmentAlreadyDeleted()
        {
            // Arrange
            var request = CreateRequest();
            SetupValidValidation(request);
            SetupAuthenticatedUser();

            var assignment = BuildAssignment();
            assignment.IsDeleted = true;

            _repoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Deleted> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentAlreadyDeleted, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenAssignmentIsAlreadyDeletedButStateIsNotWaitingForAcceptance_ShouldReturnInvalidAssignmentState()
        {
            // Arrange
            var request = CreateRequest();
            SetupValidValidation(request);
            SetupAuthenticatedUser();

            var assignment = BuildAssignment(state: AssignmentState.Accepted);
            assignment.IsDeleted = true;

            _repoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Deleted> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidAssignmentState, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenAssignmentAssetIsNull_ShouldReturnAssetOfAssignmentNotFound()
        {
            // Arrange
            var request = CreateRequest();
            SetupValidValidation(request);
            SetupAuthenticatedUser();

            var assignment = new Assignment
            {
                State = AssignmentState.WaitingForAcceptance,
                Asset = null
            };

            _repoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Deleted> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssetOfAssignmentNotFound, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenAssignmentAssetStateIsNotAssigned_ShouldReturnInvalidAssignmentAssetState()
        {
            // Arrange
            var request = CreateRequest();
            SetupValidValidation(request);
            SetupAuthenticatedUser();

            var assignment = BuildAssignment(asset: new Asset
            {
                State = AssetState.Available,
                LocationId = _locationId
            });

            _repoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Deleted> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidAssignmentAssetState, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenAssignmentAssetIsInDifferentLocation_ShouldReturnInvalidAssignmentAssetLocation()
        {
            // Arrange
            var request = CreateRequest();
            SetupValidValidation(request);
            SetupAuthenticatedUser();

            var assignment = BuildAssignment(asset: new Asset
            {
                State = AssetState.Assigned,
                LocationId = Guid.NewGuid()
            });

            _repoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Deleted> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidAssignmentAssetLocation, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenSaveChangesThrowsException_ShouldLogErrorAndReturnUnexpectedError()
        {
            // Arrange
            var request = CreateRequest();
            SetupValidValidation(request);
            SetupAuthenticatedUser();

            var assignment = BuildAssignment();

            _repoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            ErrorOr<Deleted> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnexpectedErrorOccurred, result.FirstError);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRequestIsValid_ShouldDeleteAssignmentReleaseAssetAndReturnDeleted()
        {
            // Arrange
            var request = CreateRequest();
            SetupValidValidation(request);
            SetupAuthenticatedUser();

            var assignment = BuildAssignment();

            _repoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            ErrorOr<Deleted> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Deleted, result.Value);
            Assert.True(assignment.IsDeleted);
            Assert.Equal(_utcNow, assignment.DeletedAtUtc);
            Assert.Equal(AssetState.Available, assignment.Asset!.State);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private Request CreateRequest()
        {
            return new Request(AssignmentId: Guid.NewGuid().ToString());
        }

        private void SetupValidValidation(Request request)
        {
            _validatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }

        private void SetupAuthenticatedUser()
        {
            _currentUserMock
                .Setup(x => x.IsAuthenticated)
                .Returns(true);

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns(_userId);

            _currentUserMock
                .Setup(x => x.LocationId)
                .Returns(_locationId.ToString());
        }

        private Assignment BuildAssignment(
            AssignmentState state = AssignmentState.WaitingForAcceptance,
            Asset? asset = null)
        {
            return new Assignment
            {
                State = state,
                Asset = asset ?? new Asset
                {
                    State = AssetState.Assigned,
                    LocationId = _locationId
                }
            };
        }
    }
}
