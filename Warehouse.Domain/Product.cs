using System;
using System.Collections.Generic;

namespace Warehouse.Domain
{
    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime? DateOfReceipt { get; set; }

        public List<Manufacturer> Manufacturers { get; set; }

        public Customer Customer { get; set; }
    }
}