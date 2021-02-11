using System;

namespace MongoTutorial.Core.Dtos
{
    public record ProductDto(string Id, string Name, DateTime DateOfReceipt)
    {
        public string Id { get; set; } = Id;
    }
}