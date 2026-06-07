using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Users.CanDisable
{
    public sealed class UserCanDisableSpecification : Specification<Assignment>
    {
        // Valid Assignment = (Accepted || Waiting For Acceptance) + (isDeleted = false)
        public UserCanDisableSpecification(Guid userId)
        {
            Query.Where(x =>
                x.AssignedToUserId == userId && !x.IsDeleted &&
                (
                    x.State == AssignmentState.WaitingForAcceptance ||
                    x.State == AssignmentState.Accepted
                ));
        }
    }
}