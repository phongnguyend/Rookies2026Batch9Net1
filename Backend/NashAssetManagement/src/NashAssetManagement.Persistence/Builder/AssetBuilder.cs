using System;
using System.Collections.Generic;
using System.Text;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Persistence.Builder
{
    public sealed class AssetBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _categoryId;
        private Guid _locationId;
        private string _assetName = "Asset Sample";
        private string _assetCode = "ASSET-000";
        private string _assetSpecification = "Asset Sample Specification";
        private AssetState _assetState = AssetState.Available;
        private bool _isDeleted = false;
        private DateTime _installedDateAtUtc = DateTime.UtcNow;
        private DateTime _createdAtUtc = DateTime.UtcNow;
        private DateTime? _updatedAtUtc;

        public AssetBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public AssetBuilder WithCategoryId(Guid categoryId)
        {
            _categoryId = categoryId;
            return this;
        }

        public AssetBuilder WithLocationId(Guid locationId)
        {
            _locationId = locationId;
            return this;
        }

        public AssetBuilder WithAssetName(string assetName)
        {
            _assetName = assetName;
            return this;
        }

        public AssetBuilder WithAssetCode(string assetCode)
        {
            _assetCode = assetCode;
            return this;
        }

        public AssetBuilder WithAssetSpecification(string assetSpecification)
        {
            _assetSpecification = assetSpecification;
            return this;
        }

        public AssetBuilder WithAssetState(AssetState assetState)
        {
            _assetState = assetState;
            return this;
        }

        public AssetBuilder WithIsDeleted(bool isDeleted)
        {
            _isDeleted = isDeleted;
            return this;
        }

        public AssetBuilder WithInstalledDateAtUtc(DateTime installedDateAtUtc)
        {
            _installedDateAtUtc = installedDateAtUtc;
            return this;
        }

        public AssetBuilder WithCreatedAtUtc(DateTime createdAtUtc)
        {
            _createdAtUtc = createdAtUtc;
            return this;
        }

        public AssetBuilder WithUpdatedAtUtc(DateTime? updatedAtUtc)
        {
            _updatedAtUtc = updatedAtUtc;
            return this;
        }

        public Asset Build()
        {
            return new Asset
            {
                Id = _id,
                CategoryId = _categoryId,
                LocationId = _locationId,
                Name = _assetName,
                AssetCode = _assetCode,
                Specification = _assetSpecification,
                State = _assetState,
                IsDeleted = _isDeleted,
                InstalledAtUtc = _installedDateAtUtc,
                CreatedAtUtc = _createdAtUtc,
                UpdatedAtUtc = _updatedAtUtc,
            };
        }
    }
}
