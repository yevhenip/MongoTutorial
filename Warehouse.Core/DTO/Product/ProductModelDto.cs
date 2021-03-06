using System;
using System.Collections.Generic;

namespace Warehouse.Core.DTO.Product
{
    public record ProductModelDto(string Name, DateTime DateOfReceipt, IEnumerable<string> ManufacturerIds,
        string CustomerId)
    {
    }
}