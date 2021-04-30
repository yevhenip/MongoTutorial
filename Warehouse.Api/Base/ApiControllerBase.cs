using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Api.Base
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected readonly IMediator Mediator;

        protected ApiControllerBase(IMediator mediator)
        {
            Mediator = mediator;
        }
        
        protected string UserName => User.Claims.SingleOrDefault(c => c.Type == "UserName")?.Value;
        protected string Id => User.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;
    }
}