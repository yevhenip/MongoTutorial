using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Base;
using Warehouse.Api.Logs.Commands;

namespace Warehouse.Api.Logs.Controllers.v1
{
    public class LogsController : ApiControllerBase
    {
        public LogsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await Mediator.Send(new GetLogsCommand(_ => true));
            return Ok(result.Data);
        }

        [HttpGet("actual")]
        public async Task<IActionResult> GetActualAsync()
        {
            var result = await Mediator.Send(new GetLogsCommand(l => l.ActionDate >= DateTime.UtcNow.AddDays(-1)));
            return Ok(result.Data);
        }

        [HttpGet("{logId}")]
        public async Task<IActionResult> GetAsync([FromRoute] string logId)
        {
            var result = await Mediator.Send(new GetLogCommand(logId));
            return Ok(result.Data);
        }
    }
}