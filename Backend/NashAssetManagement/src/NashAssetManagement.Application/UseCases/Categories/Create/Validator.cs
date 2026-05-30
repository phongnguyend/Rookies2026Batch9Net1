using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Categories.Create;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(20)
            .WithMessage("Category name must not exceed 20 characters.");

        RuleFor(x => x.CategoryPrefix)
            .NotEmpty().WithMessage("Category prefix is required.")
            .MaximumLength(2)
            .WithMessage("Category prefix must not exceed 2 characters.")
            .Matches("^[A-Z]+$")
            .WithMessage("Category prefix must contain uppercase letters only.");
    }
}