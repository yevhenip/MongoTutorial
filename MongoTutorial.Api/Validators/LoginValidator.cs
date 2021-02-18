using FluentValidation;
using MongoTutorial.Core.DTO.Auth;

namespace MongoTutorial.Api.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(l => l.UserName).NotEmpty()
                .WithMessage("UserName is required");
            
            RuleFor(l => l.Password).NotEmpty()
                .WithMessage("Password is required");
        }
    }
}