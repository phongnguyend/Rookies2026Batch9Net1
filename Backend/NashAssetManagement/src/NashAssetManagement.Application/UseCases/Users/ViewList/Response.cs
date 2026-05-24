using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NashAssetManagement.Application.UseCases.Users.ViewList
{
    public record Response(
        Guid Id,
        string StaffCode,
        string FullName,
        string UserName,
        string JoinedDate,
        string UserType
    )
    {
        public bool CanBeDisabled { get; set; }
    }
}