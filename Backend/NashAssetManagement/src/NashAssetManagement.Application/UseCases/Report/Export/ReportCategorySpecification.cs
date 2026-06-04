using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Report.Export
{
    public sealed class ReportCategorySpecification : Specification<Category>
    {
        public ReportCategorySpecification(Guid LocationId)
        {
            Query
                .Include(c => c.Assets.Where(a => a.LocationId == LocationId))
                .Where(c => c.Assets.Any(a => a.LocationId == LocationId))
                .AsNoTracking().AsSplitQuery();
        }
    }
}
