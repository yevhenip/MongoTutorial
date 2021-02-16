using System;
using System.Collections.Generic;
using MongoTutorial.Domain;

namespace MongoTutorial.Core.Dtos
{
    public record ProductDto(string Id = "", string Name = "", DateTime? DateOfReceipt = null,
        IEnumerable<Manufacturer> Manufacturers = null, User User = null)
    {
    }
}