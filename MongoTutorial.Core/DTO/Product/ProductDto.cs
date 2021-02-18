using System;
using System.Collections.Generic;
using MongoTutorial.Core.DTO.Users;

namespace MongoTutorial.Core.DTO.Product
{
    public record ProductDto(string Id = "", string Name = "", DateTime? DateOfReceipt = null,
        IEnumerable<Domain.Manufacturer> Manufacturers = null, UserDto User = null)
    {
    }
}