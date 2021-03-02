using FluentValidation;
using Warehouse.Core.DTO.Manufacturer;

namespace Warehouse.Api.Validators
{
    public class ManufacturerModelValidator : AbstractValidator<ManufacturerModelDto>
    {
        public ManufacturerModelValidator()
        {
            RuleFor(m => m.Name).NotEmpty()
                .WithMessage("Name required");
            
            RuleFor(m => m.Address).NotEmpty()
                .WithMessage("Address required");
        }
    }
}