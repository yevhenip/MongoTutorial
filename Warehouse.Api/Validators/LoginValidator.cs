using FluentValidation;
using Warehouse.Core.DTO.Auth;

namespace Warehouse.Api.Validators
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