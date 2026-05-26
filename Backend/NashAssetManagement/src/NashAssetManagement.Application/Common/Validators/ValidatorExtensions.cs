using FluentValidation;

namespace NashAssetManagement.Application.Common.Validators
{
    internal static class ValidatorExtensions
    {
        #region Common pagination rules

        internal static IRuleBuilderOptions<T, int?> MustBeValidPageNumber<T>(
            this IRuleBuilder<T, int?> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.")
                .When(x => ruleBuilder != null);
        }

        internal static IRuleBuilderOptions<T, int?> MustBeValidPageSize<T>(
            this IRuleBuilder<T, int?> ruleBuilder,
            int maxPageSize = 100)
        {
            return ruleBuilder
                .GreaterThan(0)
                .LessThanOrEqualTo(maxPageSize)
                .WithMessage($"Page size must be between 0 and {maxPageSize}.")
                .When(x => ruleBuilder != null);
        }

        #endregion

        #region Path Parameters

        internal static IRuleBuilderOptions<T, string?> MustBeValidGuid<T>(
            this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .Must(value => value is not null && Guid.TryParse(value, out _))
                .WithMessage("{PropertyName} must be a valid Guid/uuid.");
        }

        #endregion
    }
}