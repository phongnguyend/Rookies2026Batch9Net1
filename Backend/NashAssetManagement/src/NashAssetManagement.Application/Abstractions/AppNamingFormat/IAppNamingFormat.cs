namespace NashAssetManagement.Application.Abstractions.AppNamingFormat
{
    public interface IAppNamingFormat
    {
        string GetBaseUserName(string firstName, string lastName);
        string GetPassword(string username, DateTime dateOfBirth);
        string GetStaffCode(int number);
        string GetEmail(string username);
        string GetUniqueUserName(string baseUsername, IEnumerable<string> existingUsernames);
    }
}
