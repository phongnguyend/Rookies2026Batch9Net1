using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.GetById
{
    internal static class Errors
    {
        public static Error AssignmentNotFoundWithId(Guid id) =>
               Error.NotFound(
                   "Assignment.AssignmentNotFoundWithId",
                   $"Assignment with ID '{id}' was not found.");
    }
}
