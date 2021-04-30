using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Base;
using Warehouse.Api.Customers.Commands;
using Warehouse.Core.DTO.Customer;

namespace Warehouse.Api.Customers.Controllers.v1
{
    [Authorize]
    public class CustomersController : ApiControllerBase
    {
        public CustomersController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await Mediator.Send(new GetCustomersCommand(_ => true));
            return Ok(result.Data);
        }

        [HttpGet("{customerId:guid}")]
        public async Task<IActionResult> GetAsync([FromRoute] string customerId)
        {
            var result = await Mediator.Send(new GetCustomerCommand(customerId));
            return Ok(result.Data);
        }

        [HttpGet("{page:int}/{pageSize:int}")]
        public async Task<IActionResult> GetPageAsync([FromRoute] int page, [FromRoute] int pageSize)
        {
            var result = await Mediator.Send(new GetCustomerPageCommand(page, pageSize));
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CustomerDto customer)
        {
            var result = await Mediator.Send(new CreateCustomerCommand(customer, UserName));
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{customerId:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string customerId)
        {
            var result = await Mediator.Send(new DeleteCustomerCommand(customerId));
            return Ok(result.Data);
        }
    }
}