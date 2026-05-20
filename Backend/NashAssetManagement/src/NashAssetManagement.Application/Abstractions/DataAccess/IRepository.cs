using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Base;

namespace NashAssetManagement.Application.Abstractions.DataAccess
{
    public interface IRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void UpdateDetached(TEntity entity);
        void Delete(TEntity entity);
        Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);
        Task<List<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        Task<List<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        IQueryable<TEntity> GetQueryableSet();
    }
}
