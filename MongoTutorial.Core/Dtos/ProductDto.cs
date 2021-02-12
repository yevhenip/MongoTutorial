using System;

namespace MongoTutorial.Core.Dtos
{
    public record ProductDto(string Id = "", string Name = "", DateTime? DateOfReceipt = null)
    {
    }
}