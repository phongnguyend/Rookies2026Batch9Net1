using Ardalis.Specification;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignments;
using NashAssetManagement.Domain.Entities.Core;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.ViewUserAssignments
{
    public class HandlerTests
    {
        private readonly Mock<IRepository<Assignment, Guid>> _mockRepo;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<IValidator<Request>> _mockValidator;
        private readonly Mock<IDateTimeProvider> _mockDateTime;
        private readonly Handler _handler;

        public HandlerTests()
        {
            _mockRepo = new Mock<IRepository<Assignment, Guid>>();
            _mockUser = new Mock<ICurrentUser>();
            _mockValidator = new Mock<IValidator<Request>>();
            _mockDateTime = new Mock<IDateTimeProvider>();

            _handler = new Handler(
                _mockRepo.Object,
                _mockUser.Object,
                _mockValidator.Object,
                _mockDateTime.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var request = new Request("Name", false, 20, 1);
            var errors = new List<ValidationFailure> { new("PageSize", "Invalid page size") };

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act
            var act = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(act);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorizedError_WhenUserNotAuthenticated()
        {
            // Arrange
            var request = new Request("Name", false, 20, 1);

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnauthorizedUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedList_WhenRequestIsValidAndUserIsAuthorized()
        {
            // Arrange
            var request = new Request("Name", false, null, null);
            var userId = Guid.NewGuid();
            var fakeUtcNow = DateTime.UtcNow;
            var fakeAssignments = new List<Assignment> { new() };
            int expectedTotal = 1;

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);
            _mockDateTime.Setup(d => d.UtcNow).Returns(fakeUtcNow);
            _mockRepo
                .Setup(r => r.CountAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTotal);
            _mockRepo
                .Setup(r => r.ListAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeAssignments);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedTotal, result.Value.TotalCount);
            Assert.Equal(1, result.Value.PageNumber);
            Assert.Equal(20, result.Value.PageSize);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnidentifiedUserError_WhenCannotGetUserId()
        {

            // Arrange
            var request = new Request("Name", false, 20, 1);

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns((Guid?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnidentifiedUser, result.FirstError);
        }
    }
}
