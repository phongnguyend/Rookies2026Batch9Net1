namespace NashAssetManagement.Application.UseCases.Assignments.GetAll
{
    public record PagedQuery
    {
        public int? PageNumber { get; init; } = 1;
        public int? PageSize { get; init; } = 10;
    }
}
