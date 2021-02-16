using FluentValidation;
using MongoTutorial.Api.Models.Manufacturer;

namespace MongoTutorial.Api.Validators
{
    public class ManufacturerModelValidator : AbstractValidator<ManufacturerModel>
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