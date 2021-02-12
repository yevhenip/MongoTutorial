using System;
using FluentValidation;
using MongoTutorial.Api.Models.Product;

namespace MongoTutorial.Api.Validators
{
    public class ProductModelValidator : AbstractValidator<ProductModel>
    {
        public ProductModelValidator()
        {
            RuleFor(p => p.Name).NotEmpty()
                .WithMessage("Name is required");

            RuleFor(p => p.DateOfReceipt).LessThan(DateTime.Now)
                .WithMessage("Impossible DateTime");
        }
    }
}