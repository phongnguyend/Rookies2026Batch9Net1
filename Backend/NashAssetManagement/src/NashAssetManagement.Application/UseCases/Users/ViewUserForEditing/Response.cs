using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NashAssetManagement.Application.UseCases.Users.ViewUserForEditing
{
    public record Response(
        Guid Id,
        string FirstName,
        string LastName,
        DateTime? DateOfBirth,
        string Gender,
        DateTime JoinedDate,
        string UserType,
        bool IsCurrentUser,
        string ConcurrencyStamp
    );
}
