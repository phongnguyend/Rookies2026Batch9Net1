using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.Realtime;
using NashAssetManagement.Application.UseCases.Users.EditUser;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Auth;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using NashAssetManagement.UnitTests.TestHelpers;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.EditUser;

public class HandlerTests
{
    private static readonly Guid ExistingUserId = Guid.Parse("36c29308-4d9c-4e1b-9baf-a5dc11f26001");
    private static readonly Guid CurrentUserId = Guid.Parse("f76435a1-6f00-4a43-8c7e-dd6b95730268");
    private static readonly Guid LocationId = Guid.Parse("a3b7ef5a-bce7-401f-bbe3-94c2f7bf0b94");
    private const string ConcurrencyStamp = "concurrency-stamp";

    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<ICurrentUser> _currentUser;
    private readonly Mock<IValidator<Request>> _validator;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IRepository<RefreshToken, Guid>> _refreshTokenRepository;
    private readonly Mock<IUserSessionNotifier> _userSessionNotifier;
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
        _unitOfWork = new Mock<IUnitOfWork>();
        _refreshTokenRepository = new Mock<IRepository<RefreshToken, Guid>>();
        _userSessionNotifier = new Mock<IUserSessionNotifier>();

        _handler = new Handler(
            _userManager.Object,
            _currentUser.Object,
            _validator.Object,
            _unitOfWork.Object,
            _refreshTokenRepository.Object,
            _userSessionNotifier.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        var request = CreateValidRequest(userId: "not-a-guid");
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
        var request = CreateValidRequest();

        _currentUser.Setup(x => x.UserId).Returns((Guid?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(Errors.Unauthorized(), result.FirstError);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserHasNoLocationError_WhenCurrentUserLocationIsNull()
    {
        var request = CreateValidRequest();

        _currentUser.Setup(x => x.UserId).Returns(CurrentUserId);
        _currentUser.Setup(x => x.LocationId).Returns((string?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(Errors.UserHasNoLocation(), result.FirstError);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserWithIdNotFoundError_WhenUserDoesNotExist()
    {
        var request = CreateValidRequest();

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
        var request = CreateValidRequest();
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
    public async Task Handle_ShouldReturnAdminNotAllowedToEditOwnTypeError_WhenAdminEditsOwnType()
    {
        var request = CreateValidRequest(type: UserType.Admin);
        var user = CreateUser(userType: UserType.Staff);

        SetupCurrentUser(user.Id, LocationId.ToString());
        SetupValidValidation();
        _userManager.Setup(x => x.FindByIdAsync(request.UserId!)).ReturnsAsync(user);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(Errors.AdminNotAllowedToEditOwnType(), result.FirstError);
    }

    [Fact]
    public async Task Handle_ShouldReturnLocationMustHaveAtLeastOneAdminError_WhenDowngradingOnlyAdmin()
    {
        var request = CreateValidRequest(type: UserType.Staff);
        var user = CreateUser(userType: UserType.Admin);

        SetupCurrentUser(CurrentUserId, LocationId.ToString());
        SetupValidValidation();
        _userManager.Setup(x => x.FindByIdAsync(request.UserId!)).ReturnsAsync(user);
        _userManager
            .Setup(x => x.Users)
            .Returns(new List<User>
            {
                user,
                CreateUser(id: Guid.NewGuid(), userType: UserType.Staff)
            }.AsAsyncQueryable());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(Errors.LocationMustHaveAtLeastOneAdmin(), result.FirstError);
        _unitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _userManager.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUserWithoutForceLogout_WhenUserTypeDoesNotChange()
    {
        var request = CreateValidRequest(type: UserType.Staff);
        var user = CreateUser(userType: UserType.Staff);

        SetupCurrentUser(CurrentUserId, LocationId.ToString());
        SetupValidValidation();
        _userManager.Setup(x => x.FindByIdAsync(request.UserId!)).ReturnsAsync(user);
        _userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync([ApplicationRole.Staff]);
        _userManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.Equal(user.Id, result.Value.Id);
        Assert.Equal(request.DateOfBirth, user.DateOfBirth);
        Assert.Equal(request.Gender, user.Gender);
        Assert.Equal(request.JoinedDate, user.JoinedAtUtc);
        Assert.Equal(request.Type, user.UserType);

        _refreshTokenRepository.Verify(x => x.GetQueryableSet(), Times.Never);
        _userSessionNotifier.Verify(
            x => x.ForceLogoutAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRevokeRefreshTokensAndForceLogout_WhenUserTypeChanges()
    {
        var request = CreateValidRequest(type: UserType.Admin);
        var user = CreateUser(userType: UserType.Staff);
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(30),
            IsRevoked = false
        };

        SetupCurrentUser(CurrentUserId, LocationId.ToString());
        SetupValidValidation();
        _userManager.Setup(x => x.FindByIdAsync(request.UserId!)).ReturnsAsync(user);
        _userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync([ApplicationRole.Staff]);
        _userManager.Setup(x => x.RemoveFromRoleAsync(user, ApplicationRole.Staff)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(x => x.AddToRoleAsync(user, ApplicationRole.Admin)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(x => x.UpdateSecurityStampAsync(user)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        _refreshTokenRepository
            .Setup(x => x.GetQueryableSet())
            .Returns(new List<RefreshToken> { refreshToken }.AsAsyncQueryable());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.True(refreshToken.IsRevoked);
        Assert.NotNull(refreshToken.RevokedAtUtc);

        _unitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _userSessionNotifier.Verify(
            x => x.ForceLogoutAsync(
                user.Id,
                "Your account privilege has changed. Please login again.",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRollbackAndReturnFailedToUpdateUser_WhenUpdateFails()
    {
        var request = CreateValidRequest(type: UserType.Staff);
        var user = CreateUser(userType: UserType.Staff);

        SetupCurrentUser(CurrentUserId, LocationId.ToString());
        SetupValidValidation();
        _userManager.Setup(x => x.FindByIdAsync(request.UserId!)).ReturnsAsync(user);
        _userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync([ApplicationRole.Staff]);
        _userManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(Errors.FailedToUpdateUser(user.Id.ToString()), result.FirstError);
        _unitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
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

    private static User CreateUser(Guid? id = null, Guid? locationId = null, UserType userType = UserType.Staff)
    {
        return new User
        {
            Id = id ?? ExistingUserId,
            FirstName = "John",
            LastName = "Smith",
            DateOfBirth = new DateTime(2000, 1, 3),
            Gender = Gender.Male,
            JoinedAtUtc = new DateTime(2020, 1, 6),
            UserType = userType,
            LocationId = locationId ?? LocationId,
            ConcurrencyStamp = ConcurrencyStamp
        };
    }

    private static Request CreateValidRequest(
        string? userId = "36c29308-4d9c-4e1b-9baf-a5dc11f26001",
        DateTime? dateOfBirth = null,
        Gender gender = Gender.Male,
        DateTime? joinedDate = null,
        UserType type = UserType.Staff,
        string? concurrencyStamp = ConcurrencyStamp)
    {
        return new Request(
            userId,
            dateOfBirth ?? new DateTime(2000, 1, 3),
            gender,
            joinedDate ?? new DateTime(2020, 1, 6),
            type,
            concurrencyStamp);
    }
}
