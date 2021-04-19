using System;
using System.Globalization;

namespace Warehouse.Core.DTO.Product
{
    public record ExportProduct(string Id = "", string Name = "", DateTime DateOfReceipt = new(),
        string CustomerName = "")
    {
        public override string ToString()
        {
            return $"{Id},{Name},{DateOfReceipt.ToString(CultureInfo.InvariantCulture)},{CustomerName ?? "none"}";
        }
    }
}