using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.ReturnRequests.AdminCompleteReturnRequest;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.ReturnRequests.AdminCompleteReturnRequest
{
    public class HandlerTests
    {
        private readonly Mock<IRepository<ReturnRequest, Guid>> _repositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<Handler>> _loggerMock;
        private readonly Mock<ICurrentUser> _currentUserMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Handler _handler;

        public HandlerTests()
        {
            _repositoryMock = new Mock<IRepository<ReturnRequest, Guid>>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<Handler>>();
            _currentUserMock = new Mock<ICurrentUser>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _userManagerMock = MockUserManager();

            _handler = new Handler(
                _repositoryMock.Object,
                _uowMock.Object,
                _loggerMock.Object,
                _currentUserMock.Object,
                new Validator(),
                _userManagerMock.Object,
                _dateTimeProviderMock.Object);
        }

        [Fact]
        public async Task AdminCompleteReturnRequest_CurrentUserIdIsNull_ShouldReturnUserNotFound()
        {
            // Arrange
            var request = new Request(Guid.NewGuid().ToString());

            _currentUserMock.Setup(x => x.UserId).Returns((Guid?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UserNotFound.Code, result.FirstError.Code);

            _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AdminCompleteReturnRequest_InvalidRequest_ShouldThrowValidationException()
        {
            // Arrange
            var request = new Request("invalid-guid");

            SetupCurrentAdmin(Guid.NewGuid());

            // Act
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(request, CancellationToken.None));

            // Assert
            _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AdminCompleteReturnRequest_ReturnRequestNotFound_ShouldReturnReturnRequestNotFound()
        {
            // Arrange
            var request = new Request(Guid.NewGuid().ToString());

            SetupCurrentAdmin(Guid.NewGuid());

            _repositoryMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<Specification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReturnRequest?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ReturnRequestNotFound.Code, result.FirstError.Code);

            _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AdminCompleteReturnRequest_RequestAlreadyCompleted_ShouldReturnRequestAlreadyCompleted()
        {
            // Arrange
            var returnRequest = CreateReturnRequest(ReturnRequestState.Completed);
            var request = new Request(returnRequest.Id.ToString());

            SetupCurrentAdmin(Guid.NewGuid());

            _repositoryMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<Specification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnRequest);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.RequestAlreadyCompleted.Code, result.FirstError.Code);

            _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AdminCompleteReturnRequest_RequestCancelled_ShouldReturnRequestCancelled()
        {
            // Arrange
            var returnRequest = CreateReturnRequest(ReturnRequestState.Cancelled);
            var request = new Request(returnRequest.Id.ToString());

            SetupCurrentAdmin(Guid.NewGuid());

            _repositoryMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<Specification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnRequest);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.RequestCancelled.Code, result.FirstError.Code);

            _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AdminCompleteReturnRequest_AssignmentIsNull_ShouldReturnAssignmentNotFound()
        {
            // Arrange
            var returnRequest = new ReturnRequest
            {
                Id = Guid.NewGuid(),
                State = ReturnRequestState.WaitingForReturning,
                Assignment = null
            };

            var request = new Request(returnRequest.Id.ToString());

            SetupCurrentAdmin(Guid.NewGuid());

            _repositoryMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<Specification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnRequest);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentNotFound.Code, result.FirstError.Code);

            _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AdminCompleteReturnRequest_AssignmentIsNotAccepted_ShouldReturnInvalidAssignmentState()
        {
            // Arrange
            var returnRequest = CreateReturnRequest(
                ReturnRequestState.WaitingForReturning,
                AssignmentState.WaitingForAcceptance);

            var request = new Request(returnRequest.Id.ToString());

            SetupCurrentAdmin(Guid.NewGuid());

            _repositoryMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<Specification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnRequest);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidAssignmentState.Code, result.FirstError.Code);

            _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AdminCompleteReturnRequest_AssetIsNull_ShouldReturnAssetNotFound()
        {
            // Arrange
            var assignment = new Assignment
            {
                Id = Guid.NewGuid(),
                State = AssignmentState.Accepted,
                Asset = null
            };

            var returnRequest = new ReturnRequest
            {
                Id = Guid.NewGuid(),
                State = ReturnRequestState.WaitingForReturning,
                Assignment = assignment
            };

            var request = new Request(returnRequest.Id.ToString());

            SetupCurrentAdmin(Guid.NewGuid());

            _repositoryMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<Specification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnRequest);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssetNotFound.Code, result.FirstError.Code);

            _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AdminCompleteReturnRequest_SaveChangesThrowsException_ShouldReturnUnexpectedError()
        {
            // Arrange
            var returnRequest = CreateReturnRequest();
            var request = new Request(returnRequest.Id.ToString());

            SetupCurrentAdmin(Guid.NewGuid());

            _repositoryMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<Specification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnRequest);

            _uowMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnexpectedError.Code, result.FirstError.Code);

            _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AdminCompleteReturnRequest_ValidRequest_ShouldCompleteSuccessfully()
        {
            // Arrange
            var completedDate = new DateTime(2026, 06, 04);
            var returnRequest = CreateReturnRequest();
            var request = new Request(returnRequest.Id.ToString());

            SetupCurrentAdmin(Guid.NewGuid());

            _dateTimeProviderMock
                .Setup(x => x.UtcNow)
                .Returns(completedDate);

            _repositoryMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<Specification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnRequest);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);

            Assert.Equal(ReturnRequestState.Completed, returnRequest.State);
            Assert.Equal(completedDate.Date, returnRequest.ReturnedAtUtc);

            Assert.NotNull(returnRequest.Assignment);
            Assert.Equal(AssignmentState.Returned, returnRequest.Assignment.State);
            Assert.False(returnRequest.Assignment.IsReturning);

            Assert.NotNull(returnRequest.Assignment.Asset);
            Assert.Equal(AssetState.Available, returnRequest.Assignment.Asset.State);

            _uowMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private void SetupCurrentAdmin(Guid adminId)
        {
            _currentUserMock
                .Setup(x => x.UserId)
                .Returns(adminId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(adminId.ToString()))
                .ReturnsAsync(new User { Id = adminId });
        }

        private static Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();

            return new Mock<UserManager<User>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
        }

        private static ReturnRequest CreateReturnRequest(
            ReturnRequestState returnRequestState = ReturnRequestState.WaitingForReturning,
            AssignmentState assignmentState = AssignmentState.Accepted)
        {
            var asset = new Asset
            {
                Id = Guid.NewGuid(),
                State = AssetState.Assigned
            };

            var assignment = new Assignment
            {
                Id = Guid.NewGuid(),
                State = assignmentState,
                IsReturning = true,
                Asset = asset
            };

            return new ReturnRequest
            {
                Id = Guid.NewGuid(),
                State = returnRequestState,
                Assignment = assignment
            };
        }
    }
}