using System;

namespace Warehouse.Domain
{
    public class Log
    {
        public string Id { get; set; }
        
        public string UserName { get; set; }
        
        public string Action { get; set; }
        
        public string SerializedData { get; set; }
        
        public DateTime ActionDate { get; set; }
    }
}