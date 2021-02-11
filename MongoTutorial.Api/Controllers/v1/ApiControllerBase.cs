using Microsoft.AspNetCore.Mvc;

namespace MongoTutorial.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
    }
}