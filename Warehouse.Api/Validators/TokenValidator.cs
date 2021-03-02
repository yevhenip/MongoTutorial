using FluentValidation;
using Warehouse.Core.DTO.Auth;

namespace Warehouse.Api.Validators
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