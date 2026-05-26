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

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required");
                
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required")
                .NotEqual(x => x.OldPassword)
                .WithMessage("New password must be different from old password");
        }
    }
}
