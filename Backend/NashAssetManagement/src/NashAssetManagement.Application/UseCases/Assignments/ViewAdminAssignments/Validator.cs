using System.Globalization;
using FluentValidation;
using NashAssetManagement.Application.Common.Validators;
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

            RuleFor(x => x.SortBy)
                .Must(sortBy =>
                    string.IsNullOrEmpty(sortBy) ||
                    new[] { "assetcode", "assetname", "assignedto", "assignedby", "assigneddate", "state" }
                        .Contains(sortBy.ToLower()))
                .WithMessage((_, sortBy) =>
                    $"'{sortBy}' is not a valid sort by field. " +
                    $"Valid values are: AssetCode, AssetName, AssignedTo, AssignedBy, AssignedDate, State.");

            RuleFor(x => x.AssignedDate)
                .Must(date =>
                {
                    if (!date.HasValue) return true;
                    var currentYear = DateTime.UtcNow.Year;
                    return date.Value.Year >= currentYear - 100 &&
                           date.Value.Year <= currentYear + 100;
                })
                .WithMessage((_, date) =>
                {
                    var currentYear = DateTime.UtcNow.Year;
                    return $"'{date}' is not a valid date. " +
                           $"Year must be between {currentYear - 100} and {currentYear + 100}.";
                });

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("Search term cannot exceed 100 characters.");

            RuleFor(x => x.PageNumber)
               .MustBeValidPageNumber();

            RuleFor(x => x.PageSize)
                .MustBeValidPageSize();
        }
    }
}
