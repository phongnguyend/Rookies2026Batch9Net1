using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.ViewList;

public class GetAssetsHandler : IRequestHandler<GetAssetsRequest, ErrorOr<PagedList<GetAssetsResponse>>>
{
    private readonly IRepository<Asset, Guid> _assetRepository;
    private readonly GetAssetsValidator _validator;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<GetAssetsHandler> _logger;

    public GetAssetsHandler(
        IRepository<Asset, Guid> assetRepository,
        GetAssetsValidator validator,
        ICurrentUser currentUser,
        ILogger<GetAssetsHandler> logger
    )
    {
        _assetRepository = assetRepository;
        _validator = validator;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ErrorOr<PagedList<GetAssetsResponse>>> Handle(
        GetAssetsRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var location = Guid.Parse(_currentUser.LocationId!);
        var normalizedSearch = request.Search?.Trim();

        var categoryList = request.Categories?
                .Split(",", StringSplitOptions.RemoveEmptyEntries);

        var stateList = request.States?
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(s => Enum.Parse<AssetState>(s))
            .ToArray();

        var countSpec = new AssetCountSpec(
            categoryList,
            stateList,
            normalizedSearch,
            location);

        var totalCount = await _assetRepository.CountAsync(countSpec, cancellationToken);

        if (totalCount == 0)
            return PagedList.Create(
                new List<GetAssetsResponse>(),
                0,
                request.PageNumber,
                request.PageSize);

        var spec = new AssetListSpec(
            categoryList,
            stateList,
            normalizedSearch,
            request.SortBy,
            request.SortDirection,
            request.PageNumber,
            request.PageSize,
            location,
            request.isCreatedNewAsset);

        try
        {
            var assets = await _assetRepository.ListAsync(spec, cancellationToken);
            return PagedList.Create(assets, totalCount, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the asset.");
            return GetAssetsErrors.AssetViewList;
        }
    }
}
