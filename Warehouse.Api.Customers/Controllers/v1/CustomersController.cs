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
        
        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _customerService.GetAllAsync();
            return Ok(result.Data);
        }

        /// <summary>
        /// Gets customer based on provided id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>Customer</returns>
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetAsync([FromRoute] string customerId)
        {
            var result = await _customerService.GetAsync(customerId);
            return Ok(result.Data);
        }

        /// <summary>
        /// Creates customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Created customer</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CustomerDto customer)
        {
            var result = await _customerService.CreateAsync(customer);
            return Ok(result.Data);
        }
        
        /// <summary>
        /// Deletes customer
        /// </summary>
        /// <param name="customerId"></param>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string customerId)
        {
            var result = await _customerService.DeleteAsync(customerId);
            return Ok(result.Data);
        }
    }
}