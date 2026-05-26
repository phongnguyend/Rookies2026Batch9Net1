using ErrorOr;
using FluentValidation;
using MediatR;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignmentDetail
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
            var userId = user.UserId;
            if (userId == null) return Errors.UnidentifiedUser;

            var result = await repo.FirstOrDefaultAsync(new Spec(assignmentId, userId.Value), cancellationToken);
            if (result is null) return Errors.AssignmentNotFoundWithId(request.AssignmentId!);

            return result;
        }
    }
}
