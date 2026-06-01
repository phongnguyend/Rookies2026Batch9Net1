namespace NashAssetManagement.Application.UseCases.Users.CreateUser
{
    public record Response(
        Guid Id,
        string StaffCode,
        string UserName,
        string FirstName,
        string LastName,
        DateTime? DateOfBirth,
        DateTime JoinedDate,
        string UserType,
        string Gender
    );
}
