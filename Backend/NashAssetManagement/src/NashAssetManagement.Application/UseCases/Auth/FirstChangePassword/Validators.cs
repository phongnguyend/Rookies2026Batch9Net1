using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Auth.FirstChangePassword
{
    public class Validator : AbstractValidator<Request>
    {
        private const string NewPasswordRequired = "New password is required.";
        private const string NewPasswordLength = "New password must be at least 6 characters long.";
        private const string NewPasswordFormat = "New password must contain alphanumeric characters and have exactly one special character (@).";

        public Validator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(NewPasswordRequired)
                .MinimumLength(6).WithMessage(NewPasswordLength)
                .Matches(@"^[a-zA-Z0-9]*@[a-zA-Z0-9]*$").WithMessage(NewPasswordFormat); // avoid sql injection, only start with a-z,0-9
        }
    }
}
