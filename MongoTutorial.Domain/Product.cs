using System;
using System.Collections.Generic;

namespace MongoTutorial.Domain
{
    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime? DateOfReceipt { get; set; }

        public IEnumerable<Manufacturer> Manufacturers { get; set; }
        
        public User User { get; set; }
    }
}