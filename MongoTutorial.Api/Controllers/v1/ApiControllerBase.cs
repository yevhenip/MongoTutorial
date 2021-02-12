using Microsoft.AspNetCore.Mvc;

namespace MongoTutorial.Api.Controllers.v1
{
    // Do we really need this class?
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
    }
}