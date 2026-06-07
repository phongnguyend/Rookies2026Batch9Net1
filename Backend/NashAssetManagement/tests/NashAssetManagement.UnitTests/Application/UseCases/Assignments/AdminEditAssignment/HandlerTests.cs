using Ardalis.Specification;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assignments.AdminEditAssignment;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.AdminEditAssignment
{
    public class HandlerTests
    {
        readonly Mock<IRepository<Assignment, Guid>> _mockAssignmentRepo;
        readonly Mock<IRepository<Asset, Guid>> _mockAssetRepo;
        readonly Mock<UserManager<User>> _mockUserManager;
        readonly Mock<ICurrentUser> _mockUser;
        readonly Mock<ILogger<Handler>> _mockLogger;
        readonly Mock<IUnitOfWork> _mockUow;
        readonly Mock<IValidator<Request>> _mockValidator;
        readonly Handler _handler;

        public HandlerTests()
        {
            _mockAssignmentRepo = new Mock<IRepository<Assignment, Guid>>();
            _mockAssetRepo = new Mock<IRepository<Asset, Guid>>();
            _mockUser = new Mock<ICurrentUser>();
            _mockLogger = new Mock<ILogger<Handler>>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockValidator = new Mock<IValidator<Request>>();

            // Setup mock user store to instantiate UserManager safely
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _handler = new Handler(
                _mockAssignmentRepo.Object,
                _mockAssetRepo.Object,
                _mockUserManager.Object,
                _mockUser.Object,
                _mockLogger.Object,
                _mockUow.Object,
                _mockValidator.Object
            );
        }

        #region 1. Request, Authentication & Authorization Path Tests

        [Fact]
        public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
        {
            // Arrange
            var request = CreateDefaultRequest();
            var errors = new List<ValidationFailure> { new("AssignmentId", "ID cannot be empty") };
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
            SetupUserAuthenticated(true);
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
        [InlineData("invalid-guid")]
        public async Task Handle_WhenAdminLocationIdIsMalformed_ShouldReturnLocationNotFound(string? invalidLocationId)
        {
            // Arrange
            var request = CreateDefaultRequest();
            SetupValidationSuccess(request);
            SetupUserAuthenticated(true);
            SetupAdminRole();
            _mockUser.Setup(u => u.LocationId).Returns(invalidLocationId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.LocationNotFound, result.FirstError);
        }

        #endregion

        #region 2. Original Assignment Validation Tests

        [Fact]
        public async Task Handle_WhenAssignmentNotFound_ShouldReturnAssignmentNotFoundError()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = CreateDefaultRequest(assignmentId.ToString());
            SetupValidationSuccess(request);
            SetupUserContext(Guid.NewGuid());

            _mockAssignmentRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Assignment?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentNotFoundWithId(assignmentId.ToString()).Description, result.FirstError.Description);
        }

        [Fact]
        public async Task Handle_WhenAssignmentStateCannotBeEdited_ShouldReturnInvalidAssignmentState()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = CreateDefaultRequest(assignmentId.ToString());
            SetupValidationSuccess(request);
            SetupUserContext(Guid.NewGuid());

            var assignment = new Assignment { Id = assignmentId, State = AssignmentState.Accepted }; // Setup state so CanEdit() evaluates to false

            _mockAssignmentRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidAssignmentState, result.FirstError);
        }

        [Theory]
        [InlineData(true, false)]  // Asset location matches admin, but User location does not
        [InlineData(false, true)]  // User location matches admin, but Asset location does not
        [InlineData(false, false)] // Both locations mismatch
        public async Task Handle_WhenAdminEditsAssignmentFromOtherLocation_ShouldReturnCannotEditOtherLocationAssignment(bool assetMatches, bool userMatches)
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = CreateDefaultRequest(assignmentId.ToString());
            var adminLocationId = Guid.NewGuid();
            var skewLocationId = Guid.NewGuid();

            SetupValidationSuccess(request);
            SetupUserContext(adminLocationId);

            var assignment = new Assignment
            {
                Id = assignmentId,
                State = AssignmentState.WaitingForAcceptance, // Pass CanEdit() check
                Asset = new Asset { LocationId = assetMatches ? adminLocationId : skewLocationId },
                AssignedToUser = new User { LocationId = userMatches ? adminLocationId : skewLocationId }
            };

            _mockAssignmentRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CannotEditOtherLocationAssignment, result.FirstError);
        }

        #endregion

        #region 3. New User Validation Tests

        [Fact]
        public async Task Handle_WhenChangingUserAndNewUserNotFound_ShouldReturnNewUserNotFoundError()
        {
            // Arrange
            var oldUserId = Guid.NewGuid();
            var newUserId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var request = CreateDefaultRequest(assignmentId.ToString(), newUserId: newUserId.ToString());
            var adminLocationId = Guid.NewGuid();

            SetupValidationSuccess(request);
            SetupUserContext(adminLocationId);
            SetupDefaultValidAssignment(assignmentId, adminLocationId, currentUserId: oldUserId.ToString());

            _mockUserManager.Setup(m => m.FindByIdAsync(newUserId.ToString())).ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.NewUserNotFound(newUserId.ToString()).Description, result.FirstError.Description);
        }

        [Fact]
        public async Task Handle_WhenChangingUserAndNewUserIsDisabled_ShouldReturnCannotAssignDisabledUser()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            Guid newUserId = Guid.NewGuid();
            var oldUserId = Guid.NewGuid();
            var request = CreateDefaultRequest(assignmentId.ToString(), newUserId: newUserId.ToString());
            var adminLocationId = Guid.NewGuid();

            SetupValidationSuccess(request);
            SetupUserContext(adminLocationId);
            SetupDefaultValidAssignment(assignmentId, adminLocationId, currentUserId: oldUserId.ToString());

            var disabledUser = new User { Id = newUserId, IsDeleted = true, LocationId = adminLocationId };
            _mockUserManager.Setup(m => m.FindByIdAsync(newUserId.ToString())).ReturnsAsync(disabledUser);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CannotAssignDisabledUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenChangingUserAndNewUserBelongsToOtherLocation_ShouldReturnCannotAssignUserFromOtherLocation()
        {
            // Arrange
            Guid newUserId = Guid.NewGuid();
            Guid oldUserId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var request = CreateDefaultRequest(assignmentId.ToString(), newUserId: newUserId.ToString());
            var adminLocationId = Guid.NewGuid();

            SetupValidationSuccess(request);
            SetupUserContext(adminLocationId);
            SetupDefaultValidAssignment(assignmentId, adminLocationId, currentUserId: oldUserId.ToString());

            var nonLocalUser = new User { Id = newUserId, IsDeleted = false, LocationId = Guid.NewGuid() }; // Location mismatch
            _mockUserManager.Setup(m => m.FindByIdAsync(newUserId.ToString())).ReturnsAsync(nonLocalUser);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CannotAssignUserFromOtherLocation, result.FirstError);
        }

        #endregion

        #region 4. New Asset Validation Tests

        [Theory]
        [InlineData(null)]
        [InlineData("invalid-asset-guid")]
        public async Task Handle_WhenNewAssetIdIsMalformedGuid_ShouldReturnNewAssetNotFoundError(string? invalidAssetId)
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = CreateDefaultRequest(assignmentId.ToString(), userId.ToString(), newAssetId: invalidAssetId);
            var adminLocationId = Guid.NewGuid();

            SetupValidationSuccess(request);
            SetupUserContext(adminLocationId);

            var assignment = SetupDefaultValidAssignment(assignmentId, adminLocationId, userId.ToString());
            _mockUserManager.Setup(m => m.FindByIdAsync(assignment.AssignedToUserId.ToString())).ReturnsAsync(assignment.AssignedToUser);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.NewAssetNotFound(invalidAssetId ?? "").Description, result.FirstError.Description);
        }

        [Fact]
        public async Task Handle_WhenChangingAssetAndNewAssetNotFoundInLocation_ShouldReturnNewAssetNotFoundError()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var newAssetId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = CreateDefaultRequest(assignmentId.ToString(), userId.ToString(), newAssetId: newAssetId.ToString());
            var adminLocationId = Guid.NewGuid();

            SetupValidationSuccess(request);
            SetupUserContext(adminLocationId);

            var assignment = SetupDefaultValidAssignment(assignmentId, adminLocationId, userId.ToString());
            _mockUserManager.Setup(m => m.FindByIdAsync(assignment.AssignedToUserId.ToString())).ReturnsAsync(assignment.AssignedToUser);

            _mockAssetRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Asset>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Asset?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.NewAssetNotFound(newAssetId.ToString()).Description, result.FirstError.Description);
        }

        [Fact]
        public async Task Handle_WhenChangingAssetAndNewAssetIsNotAssignable_ShouldReturnInvalidNewAssetState()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var newAssetId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = CreateDefaultRequest(assignmentId.ToString(), userId.ToString(), newAssetId: newAssetId.ToString());
            var adminLocationId = Guid.NewGuid();

            SetupValidationSuccess(request);
            SetupUserContext(adminLocationId);

            var assignment = SetupDefaultValidAssignment(assignmentId, adminLocationId, userId.ToString());
            _mockUserManager.Setup(m => m.FindByIdAsync(assignment.AssignedToUserId.ToString())).ReturnsAsync(assignment.AssignedToUser);

            var unassignableAsset = new Asset { Id = newAssetId, State = AssetState.Assigned }; // Assigned state means IsAssignable() yields false
            _mockAssetRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Asset>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unassignableAsset);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidNewAssetState, result.FirstError);
        }

        #endregion

        #region 5. Infrastructure Failures & Happy Path Structural Tests

        [Fact]
        public async Task Handle_WhenDatabaseSaveFails_ShouldLogErrorAndReturnUnexpectedError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var request = CreateDefaultRequest(assignmentId.ToString(), userId.ToString(), assetId.ToString());
            var adminLocationId = Guid.NewGuid();

            SetupValidationSuccess(request);
            SetupUserContext(adminLocationId);

            var assignment = SetupDefaultValidAssignment(assignmentId, adminLocationId, userId.ToString(), assetId);
            _mockUserManager.Setup(m => m.FindByIdAsync(assignment.AssignedToUserId.ToString())).ReturnsAsync(assignment.AssignedToUser);
            _mockAssetRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Asset>>(), It.IsAny<CancellationToken>())).ReturnsAsync(assignment.Asset);

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database context disconnected."));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnexpectedErrorOccurred, result.FirstError);

            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenHappyPathModifyingEverything_ShouldSwapResourcesStatesAndReturnMappedResponse()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var oldAssetId = Guid.NewGuid();
            var newAssetId = Guid.NewGuid();
            var oldUserId = Guid.NewGuid();
            var newUserId = Guid.NewGuid();

            var request = CreateDefaultRequest(
                assignmentId.ToString(),
                newUserId: newUserId.ToString(),
                newAssetId: newAssetId.ToString(),
                note: "Updated specific assignment metadata details.");

            var adminLocationId = Guid.NewGuid();

            SetupValidationSuccess(request);
            SetupUserContext(adminLocationId);

            // Original Entities state configuration setup
            var originalAsset = new Asset { Id = oldAssetId, LocationId = adminLocationId, State = AssetState.Assigned, AssetCode = "AST-OLD", Name = "Laptop A" };
            var originalUser = new User { Id = oldUserId, LocationId = adminLocationId, UserName = "old.user" };
            var assignedByUser = new User { Id = Guid.NewGuid(), UserName = "admin.user" };

            var assignment = new Assignment
            {
                Id = assignmentId,
                State = AssignmentState.WaitingForAcceptance,
                AssetId = oldAssetId,
                Asset = originalAsset,
                AssignedToUserId = oldUserId,
                AssignedToUser = originalUser,
                AssignedByUser = assignedByUser,
                AssignedAtUtc = DateTime.UtcNow.AddDays(-2),
                Note = "Original placement note context."
            };

            _mockAssignmentRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Target target modifications payload structures setup 
            var targetReplacementUser = new User { Id = newUserId, IsDeleted = false, LocationId = adminLocationId, UserName = "new.user" };
            _mockUserManager.Setup(m => m.FindByIdAsync(newUserId.ToString())).ReturnsAsync(targetReplacementUser);

            var targetReplacementAsset = new Asset { Id = newAssetId, LocationId = adminLocationId, State = AssetState.Available, AssetCode = "AST-NEW", Name = "Laptop B" }; // Available state = IsAssignable() true
            _mockAssetRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Asset>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(targetReplacementAsset);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.NotNull(result.Value);

            // Verify entity structural modifications occurred cleanly 
            Assert.Equal(newUserId, assignment.AssignedToUserId);
            Assert.Equal(newAssetId, assignment.AssetId);
            Assert.Equal("Updated specific assignment metadata details.", assignment.Note);

            // CRITICAL DOMAIN STATE SWAP CHECKS
            Assert.Equal(AssetState.Available, originalAsset.State);     // Released old asset back to open inventory pool
            Assert.Equal(AssetState.Assigned, targetReplacementAsset.State); // Claimed new target asset state

            // Verify persistence occurred
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Check payload layout translation mapping integrity rules
            Assert.Equal("AST-NEW", result.Value.AssetCode);
            Assert.Equal("new.user", result.Value.AssignedTo);
            Assert.Equal("admin.user", result.Value.AssignedBy);
            Assert.Equal(AssignmentState.WaitingForAcceptance.ToString(), result.Value.State);
        }

        #endregion

        #region ⚙️ Private Setup Matrix Generation Helpers

        private Request CreateDefaultRequest(string? assignmentId = null, string? newUserId = null, string? newAssetId = null, string? note = null)
        {
            return new Request(
                AssignmentId: assignmentId ?? Guid.NewGuid().ToString(),
                Payload: new AssignmentEditPayload(
                    UserId: newUserId,
                    AssetId: newAssetId,
                    AssignedDate: DateTime.UtcNow.AddDays(2),
                    Note: note ?? "Standard test execution placement reference payload note."
                )
            );
        }

        private void SetupValidationSuccess(Request request) =>
            _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        private void SetupUserAuthenticated(bool isAuthenticated) =>
            _mockUser.Setup(u => u.IsAuthenticated).Returns(isAuthenticated);

        private void SetupAdminRole() =>
            _mockUser.Setup(u => u.Roles).Returns(new List<string> { ApplicationRole.Admin });

        private void SetupUserContext(Guid locationId)
        {
            SetupUserAuthenticated(true);
            SetupAdminRole();
            _mockUser.Setup(u => u.LocationId).Returns(locationId.ToString());
            _mockUser.Setup(u => u.UserId).Returns(Guid.NewGuid());
        }

        private Assignment SetupDefaultValidAssignment(Guid assignmentId, Guid locationId, string? currentUserId = null, Guid? currentAssetId = null)
        {
            var userId = currentUserId != null ? Guid.Parse(currentUserId!) : Guid.NewGuid();
            var assetId = currentAssetId ?? Guid.NewGuid();

            var assignment = new Assignment
            {
                Id = assignmentId,
                State = AssignmentState.WaitingForAcceptance,
                AssetId = assetId,
                Asset = new Asset { Id = assetId, LocationId = locationId, AssetCode = "AST-101", Name = "Mock Asset" },
                AssignedToUserId = userId,
                AssignedToUser = new User { Id = userId, LocationId = locationId, UserName = "current.user" },
                AssignedByUser = new User { Id = Guid.NewGuid(), UserName = "admin.user" },
                AssignedAtUtc = DateTime.UtcNow
            };

            _mockAssignmentRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            return assignment;
        }

        #endregion
    }
}
