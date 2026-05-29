using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Users.ViewUserDetail
{
    public class Validators : AbstractValidator<Request>
    {
        public Validators()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User id is required.")
                .Must(value => value is not null && Guid.TryParse(value, out _))
                .WithMessage("User id must be a valid GUID.");
        }
    }
}
