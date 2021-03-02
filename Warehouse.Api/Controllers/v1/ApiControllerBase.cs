using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
    }
}