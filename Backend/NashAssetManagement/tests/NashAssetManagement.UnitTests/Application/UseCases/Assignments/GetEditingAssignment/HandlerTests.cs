using Ardalis.Specification;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assignments.GetEditingAssignment;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.GetEditingAssignment
{
    public class HandlerTests
    {
        readonly Mock<IRepository<Assignment, Guid>> _mockRepo;
        readonly Mock<ICurrentUser> _mockUser;
        readonly Mock<IValidator<Request>> _mockValidator;
        readonly Handler _handler;

        public HandlerTests()
        {
            _mockRepo = new Mock<IRepository<Assignment, Guid>>();
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
            var request = new Request(AssignmentId: "invalid-id");
            var errors = new List<ValidationFailure> { new("AssignmentId", "Invalid Format") };

            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenUserNotAuthenticated_ShouldReturnUnauthorizedUser()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());
            SetupValidationSuccess(request);
            _mockUser.Setup(u => u.IsAuthenticated).Returns(false);

            // Act
            ErrorOr<Response> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.UnauthorizedUser, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenUserIsNotAdmin_ShouldReturnNotAdminUser()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());
            SetupValidationSuccess(request);
            SetupUserAuthenticated();
            _mockUser.Setup(u => u.Roles).Returns(new List<string> { "Staff" });

            // Act
            ErrorOr<Response> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.NotAdminUser, result.FirstError);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("not-a-valid-guid")]
        public async Task Handle_WhenUserLocationIdIsMalformed_ShouldReturnLocationNotFound(string? invalidLocationId)
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());
            SetupValidationSuccess(request);
            SetupUserAuthenticated();
            SetupAdminRole();
            _mockUser.Setup(u => u.LocationId).Returns(invalidLocationId);

            // Act
            ErrorOr<Response> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.LocationNotFound, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenAssignmentNotFound_ShouldReturnNotFoundError()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());
            SetupValidationSuccess(request);
            SetupUserAuthenticated();
            SetupAdminRole();
            _mockUser.Setup(u => u.LocationId).Returns(Guid.NewGuid().ToString());

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Assignment?)null);

            // Act
            ErrorOr<Response> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.AssignmentNotFoundWithId(request.AssignmentId!).Description, result.FirstError.Description);
        }

        [Fact]
        public async Task Handle_WhenAssignmentCannotBeEdited_ShouldReturnInvalidAssignmentState()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            SetupValidationSuccess(request);
            SetupUserAuthenticated();
            SetupAdminRole();
            _mockUser.Setup(u => u.LocationId).Returns(Guid.NewGuid().ToString());

            // Concrete instance: Set state so that CanEdit() naturally evaluates to false
            var assignment = new Assignment
            {
                Id = assignmentId,
                State = AssignmentState.Accepted
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Response> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.InvalidAssignmentState, result.FirstError);
        }

        [Theory]
        [InlineData(true, false)]  // Asset matches location, Assignee does not
        [InlineData(false, true)]  // Assignee matches location, Asset does not
        [InlineData(false, false)] // Both mismatch location
        public async Task Handle_WhenLocationCrossCheckFails_ShouldReturnCannotEditOtherLocationAssignment(bool assetMatches, bool assigneeMatches)
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var adminLocationId = Guid.NewGuid();
            var alternateLocationId = Guid.NewGuid();

            SetupValidationSuccess(request);
            SetupUserAuthenticated();
            SetupAdminRole();
            _mockUser.Setup(u => u.LocationId).Returns(adminLocationId.ToString());

            // Construct concrete dependencies with explicitly mixed LocationIds
            var assignment = new Assignment
            {
                Id = assignmentId,
                State = AssignmentState.WaitingForAcceptance,
                Asset = new Asset
                {
                    LocationId = assetMatches ? adminLocationId : alternateLocationId
                },
                AssignedToUser = new User
                {
                    LocationId = assigneeMatches ? adminLocationId : alternateLocationId
                }
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Response> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CannotEditOtherLocationAssignment, result.FirstError);
        }

        [Fact]
        public async Task Handle_WhenHappyPath_ShouldReturnMappedResponse()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var request = new Request(AssignmentId: assignmentId.ToString());
            var adminLocationId = Guid.NewGuid();

            SetupValidationSuccess(request);
            SetupUserAuthenticated();
            SetupAdminRole();
            _mockUser.Setup(u => u.LocationId).Returns(adminLocationId.ToString());

            // Fully hydrated concrete graph object setup
            var assignment = new Assignment
            {
                Id = assignmentId,
                State = AssignmentState.WaitingForAcceptance,
                AssignedAtUtc = DateTime.UtcNow,
                Note = "Testing note allocation",
                Asset = new Asset
                {
                    Id = Guid.NewGuid(),
                    AssetCode = "AST-001",
                    Name = "MacBook Pro",
                    LocationId = adminLocationId,
                    Category = new Category { CategoryName = "Laptops" }
                },
                AssignedToUser = new User
                {
                    Id = Guid.NewGuid(),
                    StaffCode = "SD-0001",
                    FirstName = "John",
                    LastName = "Doe",
                    UserType = UserType.Staff,
                    LocationId = adminLocationId
                }
            };

            _mockRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Assignment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignment);

            // Act
            ErrorOr<Response> result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.NotNull(result.Value);
            Assert.Equal(assignmentId, result.Value.Id);
            Assert.Equal("John Doe", result.Value.User.FullName);
            Assert.Equal("MacBook Pro", result.Value.Asset.AssetName);
            Assert.Equal("Laptops", result.Value.Asset.Category);
        }

        private void SetupValidationSuccess(Request request)
        {
            _mockValidator
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }

        private void SetupUserAuthenticated() => _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
        private void SetupAdminRole() => _mockUser.Setup(u => u.Roles).Returns(new List<string> { ApplicationRole.Admin });
    }
}
