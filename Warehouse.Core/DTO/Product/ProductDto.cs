using System;
using System.Collections.Generic;
using Warehouse.Core.DTO.Users;

namespace Warehouse.Core.DTO.Product
{
    public record ProductDto(string Id = "", string Name = "", DateTime? DateOfReceipt = null,
        IEnumerable<Domain.Manufacturer> Manufacturers = null, UserDto User = null)
    {
    }
}