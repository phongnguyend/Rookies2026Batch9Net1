using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.UseCases.Users.ViewUsers;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using NashAssetManagement.UnitTests.TestHelpers;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.ViewUsers;

public class HandlerTests
{
    private static readonly string LocationId = Guid.Parse("a3b7ef5a-bce7-401f-bbe3-94c2f7bf0b94").ToString();

    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<ICurrentUser> _mockUser;
    private readonly Mock<IValidator<Request>> _mockValidator;
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
        _mockUser = new Mock<ICurrentUser>();
        _mockValidator = new Mock<IValidator<Request>>();

        _handler = new Handler(
            _mockUserManager.Object,
            _mockUser.Object,
            _mockValidator.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var request = new Request(0, 10, null, null, null, null);
        var errors = new List<ValidationFailure>
        {
            new(nameof(Request.PageNumber), "Page number must be greater than 0.")
        };

        _mockUser.Setup(u => u.UserId).Returns(Guid.NewGuid());
        _mockUser.Setup(u => u.LocationId).Returns(LocationId);
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(errors));

        // Act
        var act = () => _handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorizedError_WhenCannotGetUserId()
    {
        // Arrange
        var request = new Request(1, 10, null, null, null, null);

        _mockUser.Setup(u => u.UserId).Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(Errors.Unauthorized(), result.FirstError);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserHasNoLocationError_WhenCurrentUserHasNoLocation()
    {
        // Arrange
        var request = new Request(1, 10, null, null, null, null);

        _mockUser.Setup(u => u.UserId).Returns(Guid.NewGuid());
        _mockUser.Setup(u => u.LocationId).Returns((string?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(Errors.UserHasNoLocation(), result.FirstError);
    }

    [Fact]
    public async Task Handle_ShouldExcludeDeletedUsers_WhenViewingUsers()
    {
        // Arrange
        var request = new Request(1, 10, null, null, null, null);
        var activeUser = CreateUser(
            id: Guid.Parse("4abf5324-2f18-4b84-bc2a-30ecb2de9017"),
            staffCode: "SD0001",
            isDeleted: false);
        var deletedUser = CreateUser(
            id: Guid.Parse("0e703d6f-e57d-4376-b4d6-d3927c6e8389"),
            staffCode: "SD0002",
            isDeleted: true);

        _mockUser.Setup(u => u.UserId).Returns(Guid.NewGuid());
        _mockUser.Setup(u => u.LocationId).Returns(LocationId);
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockUserManager
            .Setup(x => x.Users)
            .Returns(new List<User> { activeUser, deletedUser }.AsAsyncQueryable());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Single(result.Value.Items);
        Assert.Equal(activeUser.Id, result.Value.Items[0].Id);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_ShouldValidateCleanedRequest_WhenSearchTermHasExtraSpaces()
    {
        // Arrange
        var request = new Request(1, 10, "  Test     User  ", null, " staffCode ", null);
        Request? validatedRequest = null;

        _mockUser.Setup(u => u.UserId).Returns(Guid.NewGuid());
        _mockUser.Setup(u => u.LocationId).Returns(LocationId);
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
            .Callback<Request, CancellationToken>((r, _) => validatedRequest = r)
            .ReturnsAsync(new ValidationResult());
        _mockUserManager
            .Setup(x => x.Users)
            .Returns(new List<User>().AsAsyncQueryable());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(validatedRequest);
        Assert.Equal("Test User", validatedRequest.SearchTerm);
        Assert.Equal("staffCode", validatedRequest.SortBy);
        Assert.Equal(1, validatedRequest.PageNumber);
        Assert.Equal(10, validatedRequest.PageSize);
        _mockValidator.Verify(
            v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Response_ShouldNotExposeCanBeDisabled()
    {
        // Assert
        Assert.Null(typeof(Response).GetProperty("CanBeDisabled"));
    }

    private static User CreateUser(
        Guid id,
        string staffCode,
        bool isDeleted,
        Guid? locationId = null)
    {
        return new User
        {
            Id = id,
            StaffCode = staffCode,
            FirstName = "Test",
            LastName = "User",
            UserName = staffCode,
            JoinedAtUtc = new DateTime(2020, 1, 6),
            UserType = UserType.Staff,
            LocationId = locationId ?? Guid.Parse(LocationId),
            IsDeleted = isDeleted
        };
    }
}
