using Ardalis.Specification;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.AssetsLookupForEditAssignment;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.AssetsLookupForEditAssignment
{
    public class HandlerTests
    {
        readonly Mock<IRepository<Asset, Guid>> _mockRepo;
        readonly Mock<ICurrentUser> _mockUser;
        readonly Mock<IValidator<Request>> _mockValidator;
        readonly Handler _handler;

        public HandlerTests()
        {
            _mockRepo = new Mock<IRepository<Asset, Guid>>();
            _mockUser = new Mock<ICurrentUser>();
            _mockValidator = new Mock<IValidator<Request>>();

            _handler = new Handler(
                _mockRepo.Object,
                _mockUser.Object,
                _mockValidator.Object
            );
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
        {
            // Arrange
            var request = new Request(SearchTerm: null, SortBy: null, SortDesc: null, PageNumber: null, PageSize: null, AssignedAssetId: Guid.NewGuid().ToString());
            var errors = new List<ValidationFailure> { new("PageSize", "Invalid page size") };

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("not-a-valid-guid")]
        public async Task Handle_WhenAssignedAssetIdIsMalformedGuid_ShouldReturnInvalidAssignedAssetIdError(string? invalidAssetId)
        {
            // Arrange
            var request = new Request(SearchTerm: null, SortBy: null, SortDesc: null, PageNumber: null, PageSize: null, AssignedAssetId: invalidAssetId);
            SetupValidationSuccess();

            // Act
            ErrorOr<PagedList<Response>> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidAssignedAssetId, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenUserNotAuthenticated_ShouldReturnUnauthorizedUser()
        {
            // Arrange
            var request = new Request(SearchTerm: null, SortBy: null, SortDesc: null, PageNumber: null, PageSize: null, AssignedAssetId: Guid.NewGuid().ToString());
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
            var request = new Request(SearchTerm: null, SortBy: null, SortDesc: null, PageNumber: null, PageSize: null, AssignedAssetId: Guid.NewGuid().ToString());
            SetupValidationSuccess();
            SetupUserAuthenticated();

            _mockUser.Setup(u => u.Roles).Returns(new List<string> { "StandardUser" }); // Lacks Admin role

            // Act
            ErrorOr<PagedList<Response>> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.NotAdminUser, result.FirstError);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("bad-location-guid")]
        public async Task Handle_WhenUserLocationIdIsMalformed_ShouldReturnLocationNotFound(string? invalidLocationId)
        {
            // Arrange
            var request = new Request(SearchTerm: null, SortBy: null, SortDesc: null, PageNumber: null, PageSize: null, AssignedAssetId: Guid.NewGuid().ToString());
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

        [Fact]
        public async Task Handle_WhenHappyPath_ShouldApplySanitizationAndReturnPagedList()
        {
            // Arrange
            var request = new Request(
                SearchTerm: "  TrimMe  ",
                SortBy: " Code ",
                SortDesc: null,
                PageNumber: null,
                PageSize: null,
                AssignedAssetId: Guid.NewGuid().ToString());

            var expectedLocationId = Guid.NewGuid();
            var fakeItems = new List<Response> { new Response(Guid.NewGuid(), "", "", "") };
            int expectedTotalCount = 5;

            SetupValidationSuccess();
            SetupUserAuthenticated();
            SetupAdminRole();
            _mockUser.Setup(u => u.LocationId).Returns(expectedLocationId.ToString());

            // Mock repository behaviors for Ardalis.Specification
            _mockRepo
                .Setup(r => r.CountAsync(It.IsAny<ISpecification<Asset>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTotalCount);

            _mockRepo
                .Setup(r => r.ListAsync(It.IsAny<ISpecification<Asset, Response>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeItems);

            // Act
            ErrorOr<PagedList<Response>> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedTotalCount, result.Value.TotalCount);

            // Check fallback request sanitization values
            Assert.Equal(1, result.Value.PageNumber);  // null became fallback 1
            Assert.Equal(10, result.Value.PageSize);   // null became fallback 10

            // Ensure database pipelines are fully verified
            _mockRepo.Verify(r => r.CountAsync(It.IsAny<ISpecification<Asset>>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.ListAsync(It.IsAny<ISpecification<Asset, Response>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        private void SetupValidationSuccess()
        {
            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }

        private void SetupUserAuthenticated()
        {
            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
        }

        private void SetupAdminRole()
        {
            _mockUser.Setup(u => u.Roles).Returns(new List<string> { ApplicationRole.Admin });
        }
    }
}
