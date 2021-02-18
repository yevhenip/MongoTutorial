using System;
using System.Collections.Generic;

namespace MongoTutorial.Core.DTO.Product
{
    public record ProductModelDto(string Name, DateTime DateOfReceipt, IEnumerable<string> ManufacturerIds,
        string UserId)
    {
    }
}