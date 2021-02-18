using FluentValidation;
using MongoTutorial.Core.DTO.Auth;

namespace MongoTutorial.Api.Validators
{
    public class TokenValidator : AbstractValidator<TokenDto>
    {
        public TokenValidator()
        {
            RuleFor(t => t.Name).NotEmpty()
                .WithMessage("Token is required");
        }
    }
}