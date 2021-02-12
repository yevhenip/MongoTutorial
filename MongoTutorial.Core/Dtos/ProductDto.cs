using System;

namespace MongoTutorial.Core.Dtos
{
    // change value type to nullable
    public record ProductDto(string Id, string Name, DateTime DateOfReceipt)
    {
        public string Id { get; set; } = Id;
    }
}