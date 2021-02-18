using FluentValidation;
using MongoTutorial.Core.DTO.Manufacturer;

namespace MongoTutorial.Api.Validators
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