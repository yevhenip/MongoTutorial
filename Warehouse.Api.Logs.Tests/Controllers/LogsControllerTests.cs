using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Logs.Controllers.v1;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Logs.Tests.Controllers
{
    [TestFixture]
    public class LogsControllerTests
    {
        private LogDto _log;
        private readonly Mock<ILogService> _logService = new();
        
        private LogsController _logsController;
        
        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _logsController = new(_logService.Object);
            _log = new("a", "a", "a", "a", DateTime.Now);
        }
        
        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnsListOfLogs()
        {
            List<LogDto> logs = new(){_log};
            _logService.Setup(cs => cs.GetAllAsync())
                .ReturnsAsync(Result<List<LogDto>>.Success(logs));

            var result = await _logsController.GetAllAsync() as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(logs));
        }
        
        [Test]
        public async Task GetActualAsync_WhenCalled_ReturnsListOfLogs()
        {
            List<LogDto> logs = new(){_log};
            _logService.Setup(cs => cs.GetActualAsync())
                .ReturnsAsync(Result<List<LogDto>>.Success(logs));

            var result = await _logsController.GetActualAsync() as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(logs));
        }
        
        [Test]
        public async Task GetAsync_WhenCalled_ReturnsLog()
        {
            _logService.Setup(cs => cs.GetAsync(_log.Id))
                .ReturnsAsync(Result<LogDto>.Success(_log));

            var result = await _logsController.GetAsync(_log.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_log));
        }
    }
}