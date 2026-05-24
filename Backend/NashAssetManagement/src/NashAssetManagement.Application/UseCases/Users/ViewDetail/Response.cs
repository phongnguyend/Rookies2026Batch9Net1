

namespace NashAssetManagement.Application.UseCases.Users.ViewDetail
{
    public record Response(
        Guid Id,
        string StaffCode,
        string FullName,
        string UserName,
        string JoinedDate,
        string UserType,
        string DateOfBirth,
        string Gender,
        string Location
    );
}