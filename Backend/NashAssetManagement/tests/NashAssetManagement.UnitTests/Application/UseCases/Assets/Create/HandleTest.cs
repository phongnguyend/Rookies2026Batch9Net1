using ErrorOr;
using FluentValidation;
using Moq;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Categories.Create;
using NashAssetManagement.Application.UseCases.Categories.Specification;
using NashAssetManagement.Domain.Entities.Core;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Categories.Create;

public class HandlerTests
{
    private readonly Mock<IRepository<Category, Guid>> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateCategoryValidator _validator;
    private readonly CreateCategoryHandler _handler;

    public HandlerTests()
    {
        _categoryRepositoryMock = new Mock<IRepository<Category, Guid>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new CreateCategoryValidator();

        _handler = new CreateCategoryHandler(
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validator);
    }

    [Fact]
    public async Task Handle_Should_Create_Category_Successfully()
    {
        // Arrange
        var request = new CreateCategoryRequest(
            CategoryName: "Laptop",
            CategoryPrefix: "LA");

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

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);

        Assert.Equal("Laptop", result.Value.Name);
        Assert.Equal("LA", result.Value.Prefix);

        _categoryRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Category>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Category_Already_Exists()
    {
        // Arrange
        var request = new CreateCategoryRequest(
            CategoryName: "Laptop",
            CategoryPrefix: "LA");

        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByNameSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);

        Assert.Equal(
            CreateCategoryErrors.CategoryAlreadyExists.Code,
            result.FirstError.Code);

        _categoryRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Category>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Prefix_Already_Exists()
    {
        // Arrange
        var request = new CreateCategoryRequest(
            CategoryName: "Laptop",
            CategoryPrefix: "LA");

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
            CreateCategoryErrors.PrefixAlreadyExists.Code,
            result.FirstError.Code);

        _categoryRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Category>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Request_Is_Invalid()
    {
        // Arrange
        var request = new CreateCategoryRequest(
            CategoryName: "",
            CategoryPrefix: "");

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }
}