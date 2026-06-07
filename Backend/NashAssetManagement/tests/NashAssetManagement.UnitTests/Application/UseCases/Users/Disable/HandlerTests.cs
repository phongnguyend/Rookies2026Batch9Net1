using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Users.Disable;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using Ardalis.Specification;
using Xunit;
using NashAssetManagement.Application.Abstractions.Realtime;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.Disable
{
    public class HandlerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IRepository<Assignment, Guid>> _mockAssignmentRepository;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<IValidator<Request>> _mockValidator;
        private readonly Mock<ILogger<Handler>> _mockLogger;
        private readonly Mock<IUserSessionNotifier> _mockUserSessionNotifier;
        private readonly Handler _handler;

        public HandlerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
            _mockAssignmentRepository = new Mock<IRepository<Assignment, Guid>>();
            _mockUser = new Mock<ICurrentUser>();
            _mockValidator = new Mock<IValidator<Request>>();
            _mockLogger = new Mock<ILogger<Handler>>();
            _mockUserSessionNotifier = new Mock<IUserSessionNotifier>();

            _handler = new Handler(
                _mockUserManager.Object,
                _mockAssignmentRepository.Object,
                _mockUser.Object,
                _mockValidator.Object,
                _mockUserSessionNotifier.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ThrowValidationException()
        {
            // Arrange
            var request = new Request("not-a-guid");
            var errors = new List<ValidationFailure>
            {
                new(nameof(Request.TargetUserId), "User Id must be a valid Guid/uuid.")
            };

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act
            var act = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(act);
        }

        [Fact]
        public async Task Handle_TargetUserHasNoActiveAssignments_DisableUserSuccessfully()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var locationId = Guid.NewGuid();
            var request = new Request(targetUserId.ToString());

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(adminId);
            _mockUser.Setup(u => u.LocationId).Returns(locationId.ToString());

            var targetUser = new User { Id = targetUserId, IsDeleted = false, LocationId = locationId };

            _mockUserManager.Setup(m => m.FindByIdAsync(targetUserId.ToString())).ReturnsAsync(targetUser);
            _mockAssignmentRepository
                .Setup(r => r.AnyAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockUserManager
                .Setup(m => m.UpdateAsync(targetUser))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(targetUserId, result.Value.TargetUserId);
            Assert.True(targetUser.IsDeleted);
            _mockUserManager.Verify(m => m.UpdateAsync(targetUser), Times.Once);
        }
    }
}
