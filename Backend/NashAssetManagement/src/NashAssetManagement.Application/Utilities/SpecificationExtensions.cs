using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Base;
using System.Linq.Expressions;

namespace NashAssetManagement.Application.Utilities
{
    internal static class SpecificationExtensions
    {
        public static ISpecificationBuilder<TEntity> ApplyPaging<TEntity>(
            this ISpecificationBuilder<TEntity> builder,
            int page = 1,
            int pageSize = 10)
            where TEntity : class
        {
            return builder
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public static ISpecificationBuilder<TEntity> ApplySorting<TEntity>(
            this ISpecificationBuilder<TEntity> builder,
            string? sortBy,
            bool? sortDesc)
            where TEntity : class, ITrackable
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                // Default sort new to old
                return builder.OrderByDescending(x => x.CreatedAtUtc);
            }
            // Create the expression: x => x.Field
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, sortBy);
            var lambda = Expression.Lambda<Func<TEntity, object?>>(
                Expression.Convert(property, typeof(object)),
                parameter);

            if (sortDesc.HasValue && sortDesc.Value)
            {
                builder.OrderByDescending(lambda);
            }
            // Default sort ascending
            else
            {
                builder.OrderBy(lambda);
            }

            return builder;
        }
    }
}
