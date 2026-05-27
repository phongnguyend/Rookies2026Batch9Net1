

namespace NashAssetManagement.Application.UseCases.Users.ViewUserDetail
{
    public record Response(
        Guid Id,
        string StaffCode,
        string FullName,
        string UserName,
        DateTime JoinedDate,
        string UserType,
        DateTime? DateOfBirth,
        string Gender,
        string Location
    );
}
