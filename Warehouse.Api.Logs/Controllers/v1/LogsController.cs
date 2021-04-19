using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Controllers.v1;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Logs.Controllers.v1
{
    public class LogsController : ApiControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _logService.GetAllAsync();
            return Ok(result.Data);
        }
        
        [HttpGet("actual")]
        public async Task<IActionResult> GetActualAsync()
        {
            var result = await _logService.GetActualAsync();
            return Ok(result.Data);
        }

        [HttpGet("{logId}")]
        public async Task<IActionResult> GetAsync([FromRoute] string logId)
        {
            var result = await _logService.GetAsync(logId);
            return Ok(result.Data);
        }
    }
}