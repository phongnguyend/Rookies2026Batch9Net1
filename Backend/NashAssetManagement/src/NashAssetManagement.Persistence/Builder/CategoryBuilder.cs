using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Persistence.Builder;

public sealed class CategoryBuilder
{
    private Guid _id;
    private string _name = "TEST Category";
    private string _prefix = "TC";
    private bool _isDeleted = false;
    private DateTime? _deletedTime;

    public CategoryBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }
    public CategoryBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CategoryBuilder WithPrefix(string prefix)
    {
        _prefix = prefix;
        return this;
    }

    public CategoryBuilder WithDeleted(bool deleted)
    {
        _isDeleted = deleted;
        return this;
    }
    public CategoryBuilder WithDeletedTime(DateTime date)
    {
        _deletedTime = date;
        return this;
    }

    public Category Build()
    {
        return new Category
        {
            Id = _id,
            CategoryName = _name,
            Prefix = _prefix,
            IsDeleted = _isDeleted,
            DeletedAtUtc = _deletedTime
        };
    }
}