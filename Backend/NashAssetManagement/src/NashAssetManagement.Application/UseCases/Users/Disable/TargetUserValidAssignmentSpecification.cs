using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Users.Disable
{
    public sealed class UserValidAssignmentsSpecification : Specification<Assignment>
    {
        public UserValidAssignmentsSpecification(Guid userId)
        {
            Query.Where(x =>
                x.AssignedToUserId == userId &&
                !x.IsDeleted &&
                (
                    x.State == AssignmentState.WaitingForAcceptance ||
                    x.State == AssignmentState.Accepted
                ));
        }
    }
}