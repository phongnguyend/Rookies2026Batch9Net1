using FluentValidation;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assignments.GetAll
{
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleForEach(x => x.State)
                .Must(state =>
                    Enum.TryParse<AssignmentState>(
                        state,
                        ignoreCase: true,
                        out _))
                .WithMessage((_, state) =>
                    $"'{state}' is not a valid state. " +
                    $"Valid values are: " +
                    $"{string.Join(", ", Enum.GetNames<AssignmentState>())}.");
        }
    }
}
