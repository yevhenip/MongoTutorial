using FluentValidation;
using Warehouse.Core.DTO.Customer;

namespace Warehouse.Api.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerDto>
    {
        public CustomerValidator()
        {
            RuleFor(c => c.FullName).NotEmpty()
                .WithMessage("FullName required");

            RuleFor(c => c.Email).EmailAddress()
                .WithMessage("Enter correct Email");

            RuleFor(c => c.Phone).Matches(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")
                .WithMessage("Incorrect Phone");
        }
    }
}