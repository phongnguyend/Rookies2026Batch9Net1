using ErrorOr;
using FluentValidation;
using MediatR;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Report.View
{
    public class Handler(
        IRepository<Category, Guid> categoryRepository,
        IValidator<Request> validator
    ) : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(Request orgReq, CancellationToken cancellationToken)
        {
            // Clean Request
            var request = orgReq with
            {
                PageNumber = orgReq.PageNumber ?? AppCts.Api.PageIndex,
                PageSize = orgReq.PageSize ?? AppCts.Api.PageSize
            };

            // Validation
            var validationResults = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResults.IsValid) throw new ValidationException(validationResults.Errors);

            // Implement Logic
            var categories = await categoryRepository.ListAsync(new ReportCategorySpecification(), cancellationToken);

            var reportRows = categories.Select(category => new ReportRow(
                CategoryId: category.Id,
                CategoryName: category.CategoryName,
                Total: category.Assets.Count,
                Assigned: category.Assets.Count(a => a.State == AssetState.Assigned),
                Available: category.Assets.Count(a => a.State == AssetState.Available),
                NotAvailable: category.Assets.Count(a => a.State == AssetState.NotAvailable),
                WaitingForRecycling: category.Assets.Count(a => a.State == AssetState.WaitingForRecycling),
                Recycled: category.Assets.Count(a => a.State == AssetState.Recycled)
            ));

            reportRows = request.SortBy switch
            {
                SortBy.Category => request.SortDirection == SortDirection.Desc
                                    ? reportRows.OrderByDescending(r => r.CategoryName)
                                    : reportRows.OrderBy(r => r.CategoryName),
                SortBy.Total => request.SortDirection == SortDirection.Desc
                                    ? reportRows.OrderByDescending(r => r.Total)
                                    : reportRows.OrderBy(r => r.Total),
                SortBy.Assigned => request.SortDirection == SortDirection.Desc
                                    ? reportRows.OrderByDescending(r => r.Assigned)
                                    : reportRows.OrderBy(r => r.Assigned),
                SortBy.Available => request.SortDirection == SortDirection.Desc
                                    ? reportRows.OrderByDescending(r => r.Available)
                                    : reportRows.OrderBy(r => r.Available),
                SortBy.NotAvailable => request.SortDirection == SortDirection.Desc
                                    ? reportRows.OrderByDescending(r => r.NotAvailable)
                                    : reportRows.OrderBy(r => r.NotAvailable),
                SortBy.WaitingForRecycling => request.SortDirection == SortDirection.Desc
                                    ? reportRows.OrderByDescending(r => r.WaitingForRecycling)
                                    : reportRows.OrderBy(r => r.WaitingForRecycling),
                SortBy.Recycled => request.SortDirection == SortDirection.Desc
                                    ? reportRows.OrderByDescending(r => r.Recycled)
                                    : reportRows.OrderBy(r => r.Recycled),
                _ => reportRows.OrderBy(r => r.CategoryName)
            };

            var totalCount = reportRows.Count();

            var items = reportRows
                .Skip((request.PageNumber.Value - 1) * request.PageSize.Value)
                .Take(request.PageSize.Value)
                .ToList();

            return new Response(items, totalCount, request.PageNumber.Value, request.PageSize.Value);
        }
    }
}