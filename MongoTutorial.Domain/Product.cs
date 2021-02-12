using System;

namespace MongoTutorial.Domain
{
    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        // change value type to nullable
        public DateTime DateOfReceipt { get; set; }
    }
}