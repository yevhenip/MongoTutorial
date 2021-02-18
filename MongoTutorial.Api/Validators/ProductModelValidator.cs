using System;
using FluentValidation;
using MongoTutorial.Core.DTO.Product;

namespace MongoTutorial.Api.Validators
{
    public class ProductModelValidator : AbstractValidator<ProductModelDto>
    {
        public ProductModelValidator()
        {
            RuleFor(p => p.Name).NotEmpty()
                .WithMessage("Name is required");

            RuleFor(p => p.DateOfReceipt).LessThan(DateTime.Now)
                .WithMessage("Impossible DateTime");
            
            RuleFor(p => p.ManufacturerIds).NotNull()
                .WithMessage("Required at least manufacturer empty collection");
        }
    }
}