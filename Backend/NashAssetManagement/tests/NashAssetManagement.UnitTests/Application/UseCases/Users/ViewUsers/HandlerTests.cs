using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.UseCases.Users.ViewUsers;
using NashAssetManagement.Domain.Entities.Identity;
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
}
