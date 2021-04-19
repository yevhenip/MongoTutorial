using System;

namespace Warehouse.Core.DTO.Log
{
    public record LogDto(string Id, string UserName, string Action, string SerializedData, DateTime ActionDate);
}