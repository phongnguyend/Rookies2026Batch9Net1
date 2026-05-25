using FluentValidation;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assignments.GetAll
{
   //public class Validator : AbstractValidator<Query>
   // {
   //     public Validator()
   //     {
   //         RuleForEach(x => x.State)
   //             .Must(s => Enum.TryParse<AssignmentState>(s, ignoreCase: true, out _))
   //             .WithMessage(s => $"'{s}' is not a valid state. Valid values are: {string.Join(", ", Enum.GetNames<AssignmentState>())}.");
   //     }
   // }
}
