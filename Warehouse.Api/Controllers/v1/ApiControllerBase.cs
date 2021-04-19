using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected string UserName => User.Claims.SingleOrDefault(c => c.Type == "UserName")?.Value;
        protected string Id => User.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;
    }
}