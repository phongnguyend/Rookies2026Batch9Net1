using System.Data;
using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Users.ViewUserForEditing
{
    public class Validators : AbstractValidator<Request>
    {
        public Validators()
        {
            RuleFor(x => x.UserId)
            .MustBeValidGuid();
        }
    }
}