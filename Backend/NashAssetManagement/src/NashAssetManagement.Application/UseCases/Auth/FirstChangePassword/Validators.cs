using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Auth.FirstChangePassword
{
    public class Validator : AbstractValidator<Request>
    {
        private const string NewPasswordRequired = "New password is required.";
        private const string NewPasswordLength = "New password must be at least 6 characters long and less than 100 characters.";
        private const string NewPasswordFormat = "New password must contain at least one letter, one number, and one @ character.";

        public Validator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(NewPasswordRequired)
                .Length(6, 100).WithMessage(NewPasswordLength)
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*@)[A-Za-z\d@]+$").WithMessage(NewPasswordFormat); // avoid sql injection, only contains, start and end with a-z,0-9,@
        }
    }
}