using System.Collections.Generic;

namespace Warehouse.Core.DTO
{
    public record PageDataDto<T>(List<T> Data, long Length);
}