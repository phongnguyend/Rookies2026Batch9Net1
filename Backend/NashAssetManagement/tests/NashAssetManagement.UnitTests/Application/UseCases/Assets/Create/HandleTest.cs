using ErrorOr;
using FluentValidation;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Create;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.UseCases.Categories.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.Create;

public class HandlerTests
{
    private readonly Mock<IRepository<Asset, Guid>> _assetRepositoryMock;
    private readonly Mock<IRepository<Category, Guid>> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateAssetValidator _validator;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly CreateAssetHandler _handler;

    private readonly Guid _locationId = Guid.NewGuid();

    public HandlerTests()
    {
        _assetRepositoryMock = new Mock<IRepository<Asset, Guid>>();
        _categoryRepositoryMock = new Mock<IRepository<Category, Guid>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new CreateAssetValidator();
        _currentUserMock = new Mock<ICurrentUser>();

        _currentUserMock
            .Setup(x => x.LocationId)
            .Returns(_locationId.ToString());

        _handler = new CreateAssetHandler(
            _assetRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validator,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Asset_When_Category_Exists()
    {
        // Arrange
        var request = new CreateAssetRequest(
            AssetName: "Laptop Dell",
            Specification: "Core i7",
            InstalledDate: DateTime.UtcNow.Date,
            State: AssetState.Available,
            CategoryName: "Laptop",
            CategoryPrefix: "LA");

        var category = new CategoryDto(
            Guid.NewGuid(),
            "Laptop",
            "LA");

        _categoryRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<CategoryWithPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _assetRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<AssetMaxCodeByPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("LA000001");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal("Laptop Dell", result.Value.AssetName);
        Assert.Equal("Laptop", result.Value.Category);
        Assert.Equal(AssetState.Available, result.Value.State);

        _assetRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Asset>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Create_New_Category_When_Category_Does_Not_Exist()
    {
        // Arrange
        var request = new CreateAssetRequest(
            AssetName: "Laptop Dell",
            Specification: "Core i7",
            InstalledDate: DateTime.UtcNow.Date,
            State: AssetState.Available,
            CategoryName: "Laptop",
            CategoryPrefix: "LA");

        _categoryRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<CategoryWithPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CategoryDto?)null);

        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByNameSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _assetRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<AssetMaxCodeByPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);

        _categoryRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Category>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _assetRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Asset>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Category_Already_Exists()
    {
        // Arrange
        var request = new CreateAssetRequest(
            AssetName: "Laptop Dell",
            Specification: "Core i7",
            InstalledDate: DateTime.UtcNow.Date,
            State: AssetState.Available,
            CategoryName: "Laptop",
            CategoryPrefix: "LA");

        _categoryRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<CategoryWithPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CategoryDto?)null);

        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByNameSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(
            CreateAssetErrors.CategoryAlreadyExists.Code,
            result.FirstError.Code);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Prefix_Already_Exists()
    {
        // Arrange
        var request = new CreateAssetRequest(
            AssetName: "Laptop Dell",
            Specification: "Core i7",
            InstalledDate: DateTime.UtcNow.Date,
            State: AssetState.Available,
            CategoryName: "Laptop",
            CategoryPrefix: "LA");

        _categoryRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<CategoryWithPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CategoryDto?)null);

        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByNameSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(
            CreateAssetErrors.PrefixAlreadyExists.Code,
            result.FirstError.Code);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Request_Is_Invalid()
    {
        // Arrange
        var request = new CreateAssetRequest(
            AssetName: "",
            Specification: "",
            InstalledDate: DateTime.UtcNow.AddDays(1),
            State: AssetState.Recycled,
            CategoryName: "",
            CategoryPrefix: "");

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Location_Not_Found()
    {
        // Arrange
        var request = new CreateAssetRequest(
            AssetName: "Laptop Dell",
            Specification: "Core i7",
            InstalledDate: DateTime.UtcNow.Date,
            State: AssetState.Available,
            CategoryName: "Laptop",
            CategoryPrefix: "LA");

        _currentUserMock
            .Setup(x => x.LocationId)
            .Returns(string.Empty);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(
            CreateAssetErrors.LocationNotFound.Code,
            result.FirstError.Code);
    }

    [Fact]
    public async Task Handle_Should_Call_SaveChanges_Twice_When_New_Category_Is_Created()
    {
        // Arrange
        var request = new CreateAssetRequest(
            AssetName: "Laptop Dell",
            Specification: "Core i7",
            InstalledDate: DateTime.UtcNow.Date,
            State: AssetState.Available,
            CategoryName: "Laptop",
            CategoryPrefix: "LA");

        _categoryRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<CategoryWithPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CategoryDto?)null);

        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByNameSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _assetRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<AssetMaxCodeByPrefixSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}