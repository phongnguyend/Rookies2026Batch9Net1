using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.UseCases.Users.UsersLookup;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Identity;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.UsersLookup
{
    public class HandlerTests
    {
        readonly Mock<UserManager<User>> _mockUserManager;
        readonly Mock<ICurrentUser> _mockUser;
        readonly Mock<IValidator<Request>> _mockValidator;
        readonly Handler _handler;

        public HandlerTests()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            _mockUser = new Mock<ICurrentUser>();
            _mockValidator = new Mock<IValidator<Request>>();

            _handler = new Handler(
                _mockUserManager.Object,
                _mockUser.Object,
                _mockValidator.Object
            );
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
        {
            // Arrange
            var request = new Request(SearchTerm: null, SortBy: null, SortDesc: null, PageNumber: null, PageSize: null);
            var errors = new List<ValidationFailure> { new("PageSize", "Must be greater than 0") };

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenUserNotAuthenticated_ShouldReturnUnauthorizedUser()
        {
            // Arrange
            var request = new Request(SearchTerm: null, SortBy: null, SortDesc: null, PageNumber: null, PageSize: null);
            SetupValidationSuccess();

            _mockUser.Setup(u => u.IsAuthenticated).Returns(false);

            // Act
            ErrorOr<PagedList<Response>> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnauthorizedUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenUserIsNotAdmin_ShouldReturnNotAdminUser()
        {
            // Arrange
            var request = new Request(SearchTerm: null, SortBy: null, SortDesc: null, PageNumber: null, PageSize: null);
            SetupValidationSuccess();
            SetupUserAuthenticated();
            _mockUser.Setup(u => u.Roles).Returns(new List<string> { "Staff" });

            // Act
            ErrorOr<PagedList<Response>> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.NotAdminUser, result.FirstError);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid-guid-string")]
        public async Task Handle_WhenLocationIdIsMalformed_ShouldReturnLocationNotFound(string? invalidLocationId)
        {
            // Arrange
            var request = new Request(SearchTerm: null, SortBy: null, SortDesc: null, PageNumber: null, PageSize: null);
            SetupValidationSuccess();
            SetupUserAuthenticated();
            SetupAdminRole();
            _mockUser.Setup(u => u.LocationId).Returns(invalidLocationId);

            // Act
            ErrorOr<PagedList<Response>> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.LocationNotFound, result.FirstError);
        }

        private void SetupValidationSuccess()
        {
            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }
        
        private void SetupUserAuthenticated() => _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
        private void SetupAdminRole() => _mockUser.Setup(u => u.Roles).Returns(new List<string> { ApplicationRole.Admin });
    }
}
