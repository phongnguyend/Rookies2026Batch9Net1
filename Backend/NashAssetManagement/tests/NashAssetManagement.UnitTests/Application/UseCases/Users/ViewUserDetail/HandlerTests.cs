using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.UseCases.Users.ViewUserDetail;
using NashAssetManagement.Domain.Entities.Identity;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.ViewUserDetail;

public class HandlerTests
{
    private static readonly string LocationId = Guid.Parse("a3b7ef5a-bce7-401f-bbe3-94c2f7bf0b94").ToString();
    private static readonly Guid ExistingUserId = Guid.Parse("36c29308-4d9c-4e1b-9baf-a5dc11f26001");

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
        var request = new Request("not-a-guid");
        var errors = new List<ValidationFailure>
        {
            new(nameof(Request.UserId), "User id must be a valid GUID.")
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
        var request = new Request(ExistingUserId.ToString());

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
        var request = new Request(ExistingUserId.ToString());

        _mockUser.Setup(u => u.UserId).Returns(Guid.NewGuid());
        _mockUser.Setup(u => u.LocationId).Returns((string?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(Errors.UserHasNoLocation(), result.FirstError);
    }
}
