using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Common;
using Warehouse.Api.Logs.Commands;
using Warehouse.Api.Logs.Controllers.v1;
using Warehouse.Core.DTO.Log;

namespace Warehouse.Api.Logs.Tests.Controllers
{
    [TestFixture]
    public class LogsControllerTests
    {
        private LogDto _log;
        private readonly Mock<IMediator> _mediator = new();

        private LogsController _logsController;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _logsController = new(_mediator.Object);
            _log = new("a", "a", "a", "a", DateTime.Now);
        }

        [Test]
        public async Task GetAsync_WhenCalled_ReturnsLog()
        {
            _mediator.Setup(m => m.Send(new GetLogCommand(_log.Id), CancellationToken.None))
                .ReturnsAsync(Result<LogDto>.Success(_log));

            var result = await _logsController.GetAsync(_log.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_log));
        }
    }
}