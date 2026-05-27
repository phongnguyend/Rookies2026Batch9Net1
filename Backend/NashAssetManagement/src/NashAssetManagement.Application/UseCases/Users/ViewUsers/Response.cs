namespace NashAssetManagement.Application.UseCases.Users.ViewUsers
{
    public record Response(
        Guid Id,
        string StaffCode,
        string FullName,
        string UserName,
        DateTime JoinedDate,
        string UserType
    )
    {
        public bool CanBeDisabled { get; set; }
    }
}
