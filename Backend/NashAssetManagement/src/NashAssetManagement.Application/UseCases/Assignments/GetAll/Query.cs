using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Utilities;

namespace NashAssetManagement.Application.UseCases.Assignments.GetAll
{
    public record Query
     : PagedQuery,
       IRequest<ErrorOr<PagedList<Response>>>
    {
        public string? SearchTerm { get; init; }
        public string[]? State { get; init; }
        public string? SortBy { get; init; }
        public bool? SortDesc { get; init; }
        public DateTime? AssignedDate { get; init; }
        public bool? IncludeDeleted { get; init; }
    }
}
