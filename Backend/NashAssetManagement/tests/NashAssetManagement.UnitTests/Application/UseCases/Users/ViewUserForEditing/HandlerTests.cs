using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.UseCases.Users.ViewUserForEditing;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.ViewUserForEditing;

public class HandlerTests
{
    private static readonly Guid ExistingUserId = Guid.Parse("36c29308-4d9c-4e1b-9baf-a5dc11f26001");
    private static readonly Guid CurrentUserId = Guid.Parse("f76435a1-6f00-4a43-8c7e-dd6b95730268");
    private static readonly Guid LocationId = Guid.Parse("a3b7ef5a-bce7-401f-bbe3-94c2f7bf0b94");

    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<ICurrentUser> _currentUser;
    private readonly Mock<IValidator<Request>> _validator;
    private readonly Handler _handler;

    public HandlerTests()
    {
        var store = new Mock<IUserStore<User>>();
        _userManager = new Mock<UserManager<User>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
        _currentUser = new Mock<ICurrentUser>();
        _validator = new Mock<IValidator<Request>>();

        _handler = new Handler(
            _userManager.Object,
            _currentUser.Object,
            _validator.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        var request = new Request("not-a-guid");
        var errors = new List<ValidationFailure>
        {
            new(nameof(Request.UserId), "User Id must be a valid Guid/uuid.")
        };

        SetupCurrentUser(CurrentUserId, LocationId.ToString());
        _validator
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(errors));

        var act = () => _handler.Handle(request, CancellationToken.None);

        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorizedError_WhenCurrentUserIdIsNull()
    {
        var request = new Request(ExistingUserId.ToString());

        _currentUser.Setup(x => x.UserId).Returns((Guid?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(Errors.Unauthorized(), result.FirstError);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserHasNoLocationError_WhenCurrentUserLocationIsNull()
    {
        var request = new Request(ExistingUserId.ToString());

        _currentUser.Setup(x => x.UserId).Returns(CurrentUserId);
        _currentUser.Setup(x => x.LocationId).Returns((string?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(Errors.UserHasNoLocation(), result.FirstError);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserWithIdNotFoundError_WhenUserDoesNotExist()
    {
        var request = new Request(ExistingUserId.ToString());

        SetupCurrentUser(CurrentUserId, LocationId.ToString());
        SetupValidValidation();
        _userManager.Setup(x => x.FindByIdAsync(request.UserId!)).ReturnsAsync((User?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(Errors.UserWithIdNotFound(request.UserId!), result.FirstError);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserHasDifferentLocationError_WhenUserLocationIsDifferent()
    {
        var request = new Request(ExistingUserId.ToString());
        var user = CreateUser(locationId: Guid.NewGuid());

        SetupCurrentUser(CurrentUserId, LocationId.ToString());
        SetupValidValidation();
        _userManager.Setup(x => x.FindByIdAsync(request.UserId!)).ReturnsAsync(user);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(Errors.UserHasDifferentLocation(), result.FirstError);
        Assert.Equal(
            "You are not allowed to edit the information of users in a different location.",
            result.FirstError.Description);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserForEditing_WhenUserExistsInSameLocation()
    {
        var request = new Request(ExistingUserId.ToString());
        var user = CreateUser();

        SetupCurrentUser(CurrentUserId, LocationId.ToString());
        SetupValidValidation();
        _userManager.Setup(x => x.FindByIdAsync(request.UserId!)).ReturnsAsync(user);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.Equal(user.Id, result.Value.Id);
        Assert.Equal(user.FirstName, result.Value.FirstName);
        Assert.Equal(user.LastName, result.Value.LastName);
        Assert.Equal(user.DateOfBirth, result.Value.DateOfBirth);
        Assert.Equal(user.Gender.ToString(), result.Value.Gender);
        Assert.Equal(user.JoinedAtUtc, result.Value.JoinedDate);
        Assert.Equal(user.UserType.ToString(), result.Value.UserType);
        Assert.False(result.Value.IsCurrentUser);
    }

    [Fact]
    public async Task Handle_ShouldSetIsCurrentUserTrue_WhenRequestedUserIsCurrentUser()
    {
        var request = new Request(ExistingUserId.ToString());
        var user = CreateUser();

        SetupCurrentUser(user.Id, LocationId.ToString());
        SetupValidValidation();
        _userManager.Setup(x => x.FindByIdAsync(request.UserId!)).ReturnsAsync(user);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.True(result.Value.IsCurrentUser);
    }

    private void SetupValidValidation()
    {
        _validator
            .Setup(x => x.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupCurrentUser(Guid userId, string? locationId)
    {
        _currentUser.Setup(x => x.UserId).Returns(userId);
        _currentUser.Setup(x => x.LocationId).Returns(locationId);
    }

    private static User CreateUser(Guid? locationId = null)
    {
        return new User
        {
            Id = ExistingUserId,
            FirstName = "John",
            LastName = "Smith",
            DateOfBirth = new DateTime(2000, 1, 3),
            Gender = Gender.Male,
            JoinedAtUtc = new DateTime(2020, 1, 6),
            UserType = UserType.Staff,
            LocationId = locationId ?? LocationId
        };
    }
}
