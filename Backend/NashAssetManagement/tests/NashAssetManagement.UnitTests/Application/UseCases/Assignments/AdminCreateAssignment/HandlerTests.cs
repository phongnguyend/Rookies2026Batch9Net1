using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Assignments.AdminCreateAssignment;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using NashAssetManagement.Persistence.Builder;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.AdminCreateAssignment
{
    public class HandlerTests
    {
        private readonly Mock<IRepository<Asset, Guid>> _assetRepoMock = new();
        private readonly Mock<IRepository<Assignment, Guid>> _assignmentRepoMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IValidator<Request>> _validatorMock = new();
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<Handler>> _loggerMock = new();
        private readonly Handler _handler;

        private readonly Guid _adminId = Guid.NewGuid();
        private readonly Guid _staffId = Guid.NewGuid();
        private readonly Guid _assetId = Guid.NewGuid();
        private readonly Guid _locationId = Guid.NewGuid();
        private readonly DateTime _utcNow = DateTime.UtcNow;

        public HandlerTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);

            _dateTimeProviderMock
                .Setup(x => x.UtcNow)
                .Returns(_utcNow);

            _handler = new Handler(
                _assetRepoMock.Object,
                _assignmentRepoMock.Object,
                _currentUserMock.Object,
                _userManagerMock.Object,
                _validatorMock.Object,
                _dateTimeProviderMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);
        }

        // -------------------------------------------------------------------------
        // Validation Cases
        // -------------------------------------------------------------------------

        [Fact]
        public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
        {
            // Arrange
            var request = CreateRequest();
            var errors = new List<ValidationFailure>
        {
            new("UserId", "UserId must be a valid Guid/uuid.")
        };

            _validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(request, CancellationToken.None));

            _userManagerMock.Verify(
                x => x.FindByIdAsync(It.IsAny<string>()),
                Times.Never);

            _assignmentRepoMock.Verify(
                x => x.AddAsync(It.IsAny<Assignment>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // -------------------------------------------------------------------------
        // Auth / Current User Cases
        // -------------------------------------------------------------------------

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
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnauthorizedUser.Code, result.FirstError.Code);
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
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnidentifiedUser.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenAdminNotFoundInUserManager_ShouldReturnUnidentifiedUser()
        {
            // Arrange
            var request = CreateRequest();

            SetupValidValidation(request);
            SetupAuthenticatedUser();

            _userManagerMock
                .Setup(x => x.FindByIdAsync(_adminId.ToString()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnidentifiedUser.Code, result.FirstError.Code);
        }

        // -------------------------------------------------------------------------
        // Staff Cases
        // -------------------------------------------------------------------------

        [Fact]
        public async Task Handle_WhenStaffNotFound_ShouldReturnStaffNotFound()
        {
            // Arrange
            var request = CreateRequest();

            SetupValidValidation(request);
            SetupAuthenticatedUser();

            var admin = BuildAdmin();

            _userManagerMock
                .Setup(x => x.FindByIdAsync(_adminId.ToString()))
                .ReturnsAsync(admin);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(_staffId.ToString()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.StaffNotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenStaffIsDeleted_ShouldReturnStaffNotFound()
        {
            // Arrange
            var request = CreateRequest();

            SetupValidValidation(request);
            SetupAuthenticatedUser();

            var admin = BuildAdmin();
            var staff = BuildStaff(isDeleted: true);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(_adminId.ToString()))
                .ReturnsAsync(admin);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(_staffId.ToString()))
                .ReturnsAsync(staff);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.StaffNotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenStaffNotInSameLocation_ShouldReturnStaffNotInSameLocation()
        {
            // Arrange
            var request = CreateRequest();

            SetupValidValidation(request);
            SetupAuthenticatedUser();

            var admin = BuildAdmin();
            var staff = BuildStaff(locationId: Guid.NewGuid());

            _userManagerMock
                .Setup(x => x.FindByIdAsync(_adminId.ToString()))
                .ReturnsAsync(admin);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(_staffId.ToString()))
                .ReturnsAsync(staff);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.StaffNotInSameLocation.Code, result.FirstError.Code);
        }

        // -------------------------------------------------------------------------
        // Asset Cases
        // -------------------------------------------------------------------------

        [Fact]
        public async Task Handle_WhenAssetNotFound_ShouldReturnAssetNotFound()
        {
            // Arrange
            var request = CreateRequest();

            SetupValidValidation(request);
            SetupAuthenticatedUser();
            SetupAdminAndStaff();

            _assetRepoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<AssetSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Asset?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssetNotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenAssetNotInSameLocation_ShouldReturnAssetNotInSameLocation()
        {
            // Arrange
            var request = CreateRequest();

            SetupValidValidation(request);
            SetupAuthenticatedUser();
            SetupAdminAndStaff();

            var asset = new AssetBuilder()
                .WithId(_assetId)
                .WithLocationId(Guid.NewGuid())
                .WithAssetState(AssetState.Available)
                .Build();

            _assetRepoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<AssetSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(asset);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssetNotInSameLocation.Code, result.FirstError.Code);
        }

        [Theory]
        [InlineData(AssetState.NotAvailable)]
        [InlineData(AssetState.Assigned)]
        [InlineData(AssetState.WaitingForRecycling)]
        [InlineData(AssetState.Recycled)]
        public async Task Handle_WhenAssetIsNotAvailable_ShouldReturnAssetNotAvailable(AssetState state)
        {
            // Arrange
            var request = CreateRequest();

            SetupValidValidation(request);
            SetupAuthenticatedUser();
            SetupAdminAndStaff();

            var asset = new AssetBuilder()
                .WithId(_assetId)
                .WithLocationId(_locationId)
                .WithAssetState(state)
                .Build();

            _assetRepoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<AssetSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(asset);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssetNotAvailable.Code, result.FirstError.Code);
        }

        // -------------------------------------------------------------------------
        // Success Case
        // -------------------------------------------------------------------------

        [Fact]
        public async Task Handle_WhenRequestIsValid_ShouldCreateAssignmentAndReturnCreated()
        {
            // Arrange
            var request = CreateRequest();

            SetupValidValidation(request);
            SetupAuthenticatedUser();
            SetupAdminAndStaff();

            var asset = new AssetBuilder()
                .WithId(_assetId)
                .WithLocationId(_locationId)
                .WithAssetState(AssetState.Available)
                .Build();

            _assetRepoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<AssetSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(asset);

            _assignmentRepoMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Assignment>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);

            _assignmentRepoMock.Verify(
                x => x.AddAsync(
                    It.Is<Assignment>(a =>
                        a.AssignedByUserId == _adminId &&
                        a.AssignedToUserId == _staffId &&
                        a.AssetId == _assetId &&
                        a.State == AssignmentState.WaitingForAcceptance &&
                        a.IsReturning == false &&
                        a.CreatedAtUtc == _utcNow),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _assetRepoMock.Verify(
                x => x.UpdateDetached(
                    It.Is<Asset>(a =>
                        a.Id == _assetId &&
                        a.State == AssetState.Assigned)),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // -------------------------------------------------------------------------
        // Specification Verification
        // -------------------------------------------------------------------------

        [Fact]
        public async Task Handle_WhenRequestIsValid_ShouldCallAssetRepoWithAssetSpec()
        {
            // Arrange
            var request = CreateRequest();

            SetupValidValidation(request);
            SetupAuthenticatedUser();
            SetupAdminAndStaff();

            var asset = new AssetBuilder()
                .WithId(_assetId)
                .WithLocationId(_locationId)
                .WithAssetState(AssetState.Available)
                .Build();

            _assetRepoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<AssetSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(asset);

            _assignmentRepoMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Assignment>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assetRepoMock.Verify(
                x => x.FirstOrDefaultAsync(
                    It.IsAny<AssetSpec>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // -------------------------------------------------------------------------
        // Exception / Unexpected Error Case
        // -------------------------------------------------------------------------

        [Fact]
        public async Task Handle_WhenSaveChangesThrowsException_ShouldReturnUnexpectedErrorOccurred()
        {
            // Arrange
            var request = CreateRequest();

            SetupValidValidation(request);
            SetupAuthenticatedUser();
            SetupAdminAndStaff();

            var asset = new AssetBuilder()
                .WithId(_assetId)
                .WithLocationId(_locationId)
                .WithAssetState(AssetState.Available)
                .Build();

            _assetRepoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<AssetSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(asset);

            _assignmentRepoMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Assignment>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnexpectedErrorOccurred.Code, result.FirstError.Code);
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private Request CreateRequest()
        {
            return new Request(
                UserId: _staffId.ToString(),
                AssetId: _assetId.ToString(),
                AssignedDate: DateTime.Today.AddDays(1),
                Note: "Test note");
        }

        private void SetupValidValidation(Request request)
        {
            _validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }

        private void SetupAuthenticatedUser()
        {
            _currentUserMock
                .Setup(x => x.IsAuthenticated)
                .Returns(true);

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns(_adminId);
        }

        private User BuildAdmin()
        {
            return new User
            {
                Id = _adminId,
                LocationId = _locationId,
                IsDeleted = false
            };
        }

        private User BuildStaff(Guid? locationId = null, bool isDeleted = false)
        {
            return new User
            {
                Id = _staffId,
                LocationId = locationId ?? _locationId,
                IsDeleted = isDeleted
            };
        }

        private void SetupAdminAndStaff(Guid? staffLocationId = null, bool staffIsDeleted = false)
        {
            var admin = BuildAdmin();
            var staff = BuildStaff(staffLocationId, staffIsDeleted);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(_adminId.ToString()))
                .ReturnsAsync(admin);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(_staffId.ToString()))
                .ReturnsAsync(staff);
        }
    }
}
