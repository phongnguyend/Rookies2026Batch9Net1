using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
namespace NashAssetManagement.Persistence.Builder;

public sealed class UserBuilder
{
    private Guid _id;
    private string _userName = "TESTUSER";
    private string _staffCode = "";
    private string _firstName = "Test";
    private string _lastName = "Test";
    private DateTime? _dateOfBirth;
    private DateTime _joinDate;
    private UserType _userType;
    private Gender _gender;
    private Guid _locationId;
    private bool _isFirstLogin = false;
    private bool _isDeleted = false;
    private DateTime _isCreatedAtUtc = DateTime.UtcNow;
    private DateTime? _isDeletedAtUtc;

    public UserBuilder WithLocation(Guid locationId)
    {
        _locationId = locationId;
        return this;
    }

    public UserBuilder WithGender(Gender gender)
    {
        _gender = gender;
        return this;
    }

    public UserBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithName(string userName)
    {
        _userName = userName;
        return this;
    }

    public UserBuilder WithStaffCode(string staffCode)
    {
        _staffCode = staffCode;
        return this;
    }

    public UserBuilder WithFirsName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserBuilder WithDOB(DateTime dob)
    {
        _dateOfBirth = DateTime.SpecifyKind(dob, DateTimeKind.Utc);
        return this;
    }

    public UserBuilder WithJoinDate(DateTime joinDate)
    {
        _joinDate = DateTime.SpecifyKind(joinDate,DateTimeKind.Utc);
        return this;
    }

    public UserBuilder WithUserType(UserType userType)
    {
        _userType = userType;
        return this;
    }

    public UserBuilder WithFirstLogin(bool check)
    {
        _isFirstLogin = check;
        return this;
    }

    public UserBuilder WithIsDeleted(bool check)
    {
        _isDeleted = check;
        return this;
    }

    public UserBuilder WithIsDeletedDate(DateTime deleteDate)
    {
        _isDeletedAtUtc = DateTime.SpecifyKind(deleteDate, DateTimeKind.Utc);
        return this;
    }

    public User Build()
    {
        return new User
        {
            Id = _id,
            UserName = _userName,
            StaffCode = _staffCode,
            FirstName = _firstName,
            LastName = _lastName,
            Gender = _gender,
            IsFirstLogin = _isFirstLogin,
            DateOfBirth = _dateOfBirth,
            UserType = _userType,
            LocationId = _locationId,
            JoinedAtUtc = _joinDate,
            IsDeleted = _isDeleted,
            DeletedAtUtc = _isDeletedAtUtc,
            CreatedAtUtc = _isCreatedAtUtc
        };
    }
}
