using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Auth.Login
{
    public class Validator : AbstractValidator<Request>
    {
        private const string UsernameRequired = "Username is required.";
        private const string PasswordRequired = "Password is required.";
        private const string UsernameFormat = "Username must contain only letters, digits and -._@+ characters.";

        public Validator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage(UsernameRequired)
                .Matches("^[a-zA-Z0-9-._@+]+$").WithMessage(UsernameFormat);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(PasswordRequired);
        }
    }
}
