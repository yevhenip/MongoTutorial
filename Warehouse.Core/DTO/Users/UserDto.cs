using System;
using System.Collections.Generic;

namespace Warehouse.Core.DTO.Users
{
    public record UserDto(
        string Id = "", 
        string UserName = "", 
        string FullName = "", 
        string PasswordHash = "",
        DateTime RegistrationDateTime = default, 
        string Email = "", 
        string Phone = "", 
        IEnumerable<string> Roles = null,
        string SessionId = null)
    {
    }
}