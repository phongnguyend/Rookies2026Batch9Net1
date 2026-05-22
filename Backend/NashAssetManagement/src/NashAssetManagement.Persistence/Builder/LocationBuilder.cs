using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Persistence.Builder;

public sealed class LocationBuilder
{
    private Guid _id;
    private string _name = "Test Location";
    private string _prefix = "TL";
    private bool _isDeleted = false;
    private DateTime? _deletedTime;
    public LocationBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }
    public LocationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public LocationBuilder WithPrefix(string prefix)
    {
        _prefix = prefix;
        return this;
    }

    public LocationBuilder WithDeleted(bool deleted)
    {
        _isDeleted = deleted;
        return this;
    }
    public LocationBuilder WithDeletedDate(DateTime date)
    {
        _deletedTime = date;
        return this;
    }
    public Location Build()
    {
        return new Location
        {
            Id = _id,
            Name = _name,
            Prefix = _prefix,
            IsDeleted = _isDeleted,
            DeletedAtUtc = _deletedTime,
        };
    }
}