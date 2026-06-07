using ErrorOr;
using FluentValidation;
using MediatR;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.GetEditingAssignment
{
    internal class Handler(
        IRepository<Assignment, Guid> repo,
        ICurrentUser user,
        IValidator<Request> validator)
        : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
            var assignmentId = Guid.Parse(request.AssignmentId!);

            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;
            if (!user.Roles.Contains(ApplicationRole.Admin)) return Errors.NotAdminUser;
            if (!Guid.TryParse(user.LocationId, out Guid userLocationId)) return Errors.LocationNotFound;

            var assignment = await repo.FirstOrDefaultAsync(new Spec(assignmentId), cancellationToken);
            if (assignment == null) return Errors.AssignmentNotFoundWithId(request.AssignmentId!);
            if (!assignment.CanEdit()) return Errors.InvalidAssignmentState;

            // Check whether admin is trying to edit assignment of other location
            // Since assignment does not have location Id, we compare user's location Id with location Ids
            // from assignment's asset and assignee. This is sufficient.
            if (userLocationId != assignment.Asset!.LocationId || userLocationId != assignment.AssignedToUser!.LocationId)
                return Errors.CannotEditOtherLocationAssignment;

            return new Response(assignment.Id,
                new Assignee(
                    assignment.AssignedToUser!.Id,
                    assignment.AssignedToUser!.StaffCode,
                    assignment.AssignedToUser!.GetFullName(),
                    assignment.AssignedToUser.UserType.ToString()),
                new AssignmentAsset(
                    assignment.Asset!.Id,
                    assignment.Asset!.AssetCode,
                    assignment.Asset!.Name,
                    assignment.Asset!.Category!.CategoryName),
                assignment.AssignedAtUtc,
                assignment.Note);
        }
    }
}
