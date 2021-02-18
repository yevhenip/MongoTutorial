using FluentValidation;
using MongoTutorial.Core.DTO.Users;

namespace MongoTutorial.Api.Validators
{
    public class UserModelValidator : AbstractValidator<UserModelDto>
    {
        public UserModelValidator()
        {
            RuleFor(r => r.Email).NotEmpty().EmailAddress()
                .WithMessage("Input your Email like example@example.com");

            RuleFor(r => r.UserName).MaximumLength(15)
                .WithMessage("UserName should be less than 15 symbols");
            RuleFor(r => r.UserName).NotEmpty().MinimumLength(5)
                .WithMessage("UserName should be greater than 5 symbols");

            RuleFor(r => r.FullName).MaximumLength(55)
                .WithMessage("Full Name should be less than 55 symbols");
            RuleFor(r => r.FullName).NotEmpty().MinimumLength(5)
                .WithMessage("UserName should be greater than 5 symbols");

            RuleFor(r => r.Phone).Matches(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")
                .WithMessage("Incorrect Phone");
        }
    }
}