using Ardalis.Specification;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignmentDetail;
using NashAssetManagement.Domain.Entities.Core;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.ViewUserAssignmentDetail
{
    public class HandlerTests
    {
        private readonly Mock<IRepository<Assignment, Guid>> _mockRepo;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<IValidator<Request>> _mockValidator;
        private readonly Handler _handler;

        public HandlerTests()
        {
            // Shared Arrange: Initialize dependencies
            _mockRepo = new Mock<IRepository<Assignment, Guid>>();
            _mockUser = new Mock<ICurrentUser>();
            _mockValidator = new Mock<IValidator<Request>>();

            _handler = new Handler(_mockRepo.Object, _mockUser.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
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
        public async Task Handle_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(false);

            // Act
            ErrorOr<Response> result = await _handler.Handle(request, CancellationToken.None);

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
            ErrorOr<Response> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnidentifiedUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFoundError_WhenAssignmentDoesNotExist()
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
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Response?)null);

            // Act
            ErrorOr<Response> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentNotFoundWithId(request.AssignmentId!).Description, result.FirstError.Description);
            Assert.Equal(Errors.AssignmentNotFoundWithId(request.AssignmentId!).Type, result.FirstError.Type);
        }

        [Fact]
        public async Task Handle_ShouldReturnResponse_WhenAssignmentExistsAndUserAuthorized()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var expectedResponse = new Response(Guid.NewGuid(), "", "", "", "", "", DateTime.UtcNow, "", "");

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(Guid.NewGuid());

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            ErrorOr<Response> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(expectedResponse, result.Value);
        }
    }
}
