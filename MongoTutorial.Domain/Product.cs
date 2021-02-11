using System;

namespace MongoTutorial.Domain
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfReceipt { get; set; }
    }
}