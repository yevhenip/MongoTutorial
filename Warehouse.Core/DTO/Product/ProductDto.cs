using System;
using System.Collections.Generic;
using Warehouse.Core.DTO.Customer;

namespace Warehouse.Core.DTO.Product
{
    public record ProductDto(string Id = "", string Name = "", DateTime? DateOfReceipt = null,
        IEnumerable<Domain.Manufacturer> Manufacturers = null, CustomerDto Customer = null)
    {
    }
}