using ErrorOr;
using FluentValidation;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.UseCases.Assets.ViewDetail;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.ViewDetail;

public class HandlerTests
{
    private readonly Mock<IRepository<Asset, Guid>> _assetRepositoryMock;
    private readonly GetAssetDetailValidator _validator;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly GetAssetDetailHandler _handler;

    private readonly Guid _assetId = Guid.NewGuid();
    private readonly Guid _locationId = Guid.NewGuid();

    public HandlerTests()
    {
        _assetRepositoryMock = new Mock<IRepository<Asset, Guid>>();
        _validator = new GetAssetDetailValidator();
        _currentUserMock = new Mock<ICurrentUser>();

        _currentUserMock
            .Setup(u => u.LocationId)
            .Returns(_locationId.ToString());

        _handler = new GetAssetDetailHandler(
            _assetRepositoryMock.Object,
            _validator,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_AssetDetail_When_Asset_Exists()
    {
        var request = new GetAssetDetailRequest(_assetId.ToString());

        var expectedResponse = new GetAssetDetailResponse(
            Id: _assetId,
            AssetCode: "LA000001",
            Name: "Laptop Dell XPS",
            Specification: "Core i7, 16GB RAM",
            InstalledAtUtc: DateTime.UtcNow,
            State: AssetState.Available,
            Category: "Laptop",
            Location: "Ho Chi Minh",
            History:
            [
                new GetAssetAssignmentHistoryResponse(    // ← correct type name
                    AssignedAtUtc: new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc),
                    AssignedTo: "Nhi Ho Thi",
                    AssignedBy: "An Nguyen Hoang",
                    ReturnedAtUtc: new DateTime(2026, 1, 25, 0, 0, 0, DateTimeKind.Utc))
            ]);

        _assetRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<AssetDetailSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.Equal(expectedResponse.AssetCode, result.Value.AssetCode);
        Assert.Equal(expectedResponse.Name, result.Value.Name);
        Assert.Equal(expectedResponse.Category, result.Value.Category);
        Assert.Single(result.Value.History);                                                        // ← history has 1 item
        Assert.Equal("Nhi Ho Thi", result.Value.History.First().AssignedTo);
        Assert.Equal("An Nguyen Hoang", result.Value.History.First().AssignedBy);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Asset_Does_Not_Exist()
    {
        var request = new GetAssetDetailRequest(_assetId.ToString());

        _assetRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<AssetDetailSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetAssetDetailResponse?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
        Assert.Equal(GetAssetDetailErrors.NotFound(_assetId).Code, result.FirstError.Code);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Id_Is_Empty()
    {
        var request = new GetAssetDetailRequest(string.Empty);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Call_Repository_Once_When_Request_Is_Valid()
    {
        var request = new GetAssetDetailRequest(_assetId.ToString());

        _assetRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<AssetDetailSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetAssetDetailResponse?)null);

        await _handler.Handle(request, CancellationToken.None);

        _assetRepositoryMock.Verify(
            r => r.FirstOrDefaultAsync(
                It.IsAny<AssetDetailSpec>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Not_Call_Repository_When_Validation_Fails()
    {
        var request = new GetAssetDetailRequest(string.Empty);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));

        _assetRepositoryMock.Verify(
            r => r.FirstOrDefaultAsync(
                It.IsAny<AssetDetailSpec>(),
                It.IsAny<CancellationToken>()),
            Times.Never);  
    }
}