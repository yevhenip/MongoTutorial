// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using AutoMapper;
// using EasyNetQ;
// using Microsoft.Extensions.Options;
// using Moq;
// using NUnit.Framework;
// using Warehouse.Api.Logs.Business;
// using Warehouse.Core.Common;
// using Warehouse.Core.DTO.Log;
// using Warehouse.Core.Interfaces.Repositories;
// using Warehouse.Core.Settings;
// using Warehouse.Domain;
//
// namespace Warehouse.Api.Logs.Tests.Business
// {
//     [TestFixture]
//     public class LogBusinessTests
//     {
//         private LogService _logService;
//
//         private readonly LogDto _log = new("a", "a", "a", "a", DateTime.Now);
//
//         private readonly Log _dbLog = new()
//             {Id = "a", Action = "a", UserName = "a", SerializedData = "a", ActionDate = DateTime.Now};
//
//         private readonly Mock<IOptions<PollySettings>> _pollyOptions = new();
//         private readonly Mock<ILogRepository> _logRepository = new();
//         private readonly Mock<IMapper> _mapper = new();
//         private readonly Mock<IBus> _bus = new();
//
//         [OneTimeSetUp]
//         public void SetUpOnce()
//         {
//             _pollyOptions.Setup(opt => opt.Value).Returns(new PollySettings {RepeatedTimes = 2, RepeatedDelay = 3});
//             _logService = new(_logRepository.Object, _mapper.Object, _bus.Object, _pollyOptions.Object);
//         }
//
//         [Test]
//         public async Task GetAllAsync_WhenCalled_ReturnsListOfLogs()
//         {
//             var logs = ConfigureGetAll();
//
//             var result = await _logService.GetAllAsync();
//
//             Assert.That(result.Data, Is.EqualTo(logs));
//         }
//
//         [Test]
//         public async Task GetAsync_WhenCalledWithExistingId_ReturnsLog()
//         {
//             ConfigureGet(_dbLog);
//
//             var result = await _logService.GetAsync(_dbLog.Id);
//
//             Assert.That(result.Data, Is.EqualTo(_log));
//         }
//
//         [Test]
//         public void GetAsync_WhenCalledWithUnExistingId_ThrowsResultOfLogException()
//         {
//             ConfigureGet(null);
//
//             Assert.ThrowsAsync<Result<Log>>(
//                 async () => await _logService.GetAsync(_dbLog.Id));
//         }
//
//         [Test]
//         public async Task GetActualAsync_WhenCalled_ReturnsLog()
//         {
//             var logs = ConfigureGetAll();
//
//             var result = await _logService.GetActualAsync();
//
//             Assert.That(result.Data, Is.EqualTo(logs));
//         }
//
//         [Test]
//         public async Task CreateAsync_WhenCalled()
//         {
//             ConfigureCreate();
//
//             await _logService.CreateAsync(_log);
//
//             Assert.IsTrue(true);
//         }
//
//         private List<LogDto> ConfigureGetAll()
//         {
//             List<LogDto> logs = new() {_log};
//             List<Log> logsFromDb = new() {_dbLog};
//             _logRepository.Setup(cr => cr.GetRangeAsync(_ => true)).ReturnsAsync(logsFromDb);
//             _logRepository.Setup(cr => cr.GetRangeAsync(l => l.ActionDate >= DateTime.UtcNow.AddDays(-1)))
//                 .ReturnsAsync(logsFromDb);
//             _mapper.Setup(m => m.Map<List<LogDto>>(logsFromDb)).Returns(logs);
//             return logs;
//         }
//
//         private void ConfigureGet(Log dbLog)
//         {
//             _logRepository.Setup(cr => cr.GetAsync(l => l.Id == _dbLog.Id)).ReturnsAsync(dbLog);
//             _mapper.Setup(m => m.Map<LogDto>(_dbLog)).Returns(_log);
//         }
//
//         private void ConfigureCreate()
//         {
//             _mapper.Setup(m => m.Map<LogDto>(_log)).Returns(_log);
//             _mapper.Setup(m => m.Map<Log>(_log)).Returns(_dbLog);
//         }
//     }
// }