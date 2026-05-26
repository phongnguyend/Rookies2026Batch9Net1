using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Auth.ChangePassword
{
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage("Old password is required");

            // Validation rules aligned with the Identity password policy configuration
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required")
                .MinimumLength(6)
                .WithMessage("New password must be at least 6 characters")
                .Matches("[a-z]")
                .WithMessage("New password must contain at least one lowercase letter")
                .Matches("[0-9]")
                .WithMessage("New password must contain at least one digit")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("New password must contain at least one non-alphanumeric character")
                .NotEqual(x => x.OldPassword)
                .WithMessage("New password must be different from old password");
        }
    }
}
