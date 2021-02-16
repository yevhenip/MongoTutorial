using System;
using System.Collections.Generic;

namespace MongoTutorial.Api.Models.Product
{
    public record ProductModel(string Name, DateTime DateOfReceipt, IEnumerable<string> ManufacturerIds)
    {
    }
}