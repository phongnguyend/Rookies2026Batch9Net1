using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Users.CanDisable;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using Ardalis.Specification;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.CanDisable
{
    public class HandlerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IRepository<Assignment, Guid>> _mockAssignmentRepository;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<IValidator<Request>> _mockValidator;
        private readonly Mock<ILogger<Handler>> _mockLogger;
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

            _handler = new Handler(
                _mockUserManager.Object,
                _mockAssignmentRepository.Object,
                _mockUser.Object,
                _mockValidator.Object,
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
                new(nameof(Request.UserId), "User Id must be a valid Guid/uuid.")
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
        public async Task Handle_TargetUserHasNoActiveAssignments_ReturnCanDisableTrue()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var request = new Request(targetUserId.ToString());

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(adminId);
            _mockUser.Setup(u => u.Username).Returns("admin");

            var adminUser = new User { Id = adminId };
            var targetUser = new User { Id = targetUserId };

            _mockUserManager.Setup(m => m.FindByIdAsync(adminId.ToString())).ReturnsAsync(adminUser);
            _mockUserManager.Setup(m => m.IsInRoleAsync(adminUser, ApplicationRole.Admin)).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.FindByIdAsync(targetUserId.ToString())).ReturnsAsync(targetUser);

            _mockAssignmentRepository
                .Setup(r => r.AnyAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(targetUserId, result.Value.TargetUserId);
            Assert.True(result.Value.CanDisable);
        }
    }
}
