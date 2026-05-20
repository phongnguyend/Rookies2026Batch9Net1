namespace NashAssetManagement.Domain.Entities.Base
{
    public abstract class BaseEntity<TKey> : IHasKey<TKey>
    {
        public TKey Id { get; set; } = default!;
    }
}
