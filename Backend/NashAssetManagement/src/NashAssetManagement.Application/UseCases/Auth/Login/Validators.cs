using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Auth.Login
{
    public class Validator : AbstractValidator<Request>
    {
        private const string UsernameRequired = "Username is required.";
        private const string PasswordRequired = "Password is required.";
        private const string UsernameLength = "Username must not exceed 100 characters.";
        private const string UsernameFormat = "Username must contain only letters.";

        public Validator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage(UsernameRequired)
                .MaximumLength(100).WithMessage(UsernameLength)
                .Matches("^[a-zA-Z]+$").WithMessage(UsernameFormat);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(PasswordRequired);
        }
    }
}
