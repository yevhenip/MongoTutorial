using System;

namespace MongoTutorial.Api.Models.Product
{
    public record ProductModel(string Name, DateTime DateOfReceipt)
    {
    }
}