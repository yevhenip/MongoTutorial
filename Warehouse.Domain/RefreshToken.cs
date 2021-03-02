using System;

namespace Warehouse.Domain
{
    public class RefreshToken
    {
        public string Id { get; set; }
        
        public string Token { get; set; }
        
        public DateTime DateCreated { get; set; }
        
        public DateTime DateExpires { get; set; }

        public User User { get; set; }
    }
}