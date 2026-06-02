using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Report.View
{
    public sealed class ReportCategorySpecification : Specification<Category>
    {
        public ReportCategorySpecification()
        {
            Query.Include(c => c.Assets).AsNoTracking().AsSplitQuery();
        }
    }
}
