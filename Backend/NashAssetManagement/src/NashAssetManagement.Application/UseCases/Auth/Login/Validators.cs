using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Auth.Login
{
    public class Validator : AbstractValidator<Request>
    {
        public const string UsernameRequired = "Username is required.";
        public const string PasswordRequired = "Password is required.";
        public const string UsernameLength = "Username must not exceed 100 characters.";
        public const string UsernameFormat = "Username must contain only letters and digits.";

        public Validator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage(UsernameRequired)
                .MaximumLength(100).WithMessage(UsernameLength)
                .Matches("^[a-zA-Z0-9]+$").WithMessage(UsernameFormat);

            RuleFor(x => x.Password)
                .NotNull().WithMessage(PasswordRequired)
                .Must(x => x == null || x.Length > 0).WithMessage(PasswordRequired);
        }
    }
}
