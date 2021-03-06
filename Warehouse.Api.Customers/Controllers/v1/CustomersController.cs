using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Controllers.v1;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Customers.Controllers.v1
{
    [Authorize]
    public class CustomersController : ApiControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _customerService.GetAllAsync();
            return Ok(result.Data);
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetAsync([FromRoute] string customerId)
        {
            var result = await _customerService.GetAsync(customerId);
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CustomerDto customer)
        {
            var result = await _customerService.CreateAsync(customer);
            return Ok(result.Data);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string customerId)
        {
            var result = await _customerService.DeleteAsync(customerId);
            return Ok(result.Data);
        }
    }
}