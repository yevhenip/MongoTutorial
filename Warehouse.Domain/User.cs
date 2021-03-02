using System;
using System.Collections.Generic;

namespace Warehouse.Domain
{
    public class User
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime RegistrationDateTime { get; set; }

        public IEnumerable<string> Roles { get; set; } = new List<string>();

        public string SessionId { get; set; }
    }
}