using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Report.View
{
    public sealed class ReportCategorySpecification : Specification<Category>
    {
        public ReportCategorySpecification(Guid LocationId)
        {
            // Only get from the the assets that are in the location of the same Admin
            Query
                .Include(c => c.Assets)
                .Where(c => c.Assets.Any(a => a.LocationId == LocationId))
                .AsNoTracking().AsSplitQuery();
        }
    }
}
