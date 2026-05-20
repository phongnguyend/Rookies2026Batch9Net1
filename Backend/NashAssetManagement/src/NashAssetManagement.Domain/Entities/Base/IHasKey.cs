namespace NashAssetManagement.Domain.Entities.Base
{
    public interface IHasKey<TKey>
    {
        TKey Id { get; set; }
    }
}
