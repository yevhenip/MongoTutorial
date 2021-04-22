using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EasyNetQ;
using Microsoft.Extensions.Options;
using Warehouse.Api.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings;
using Warehouse.Domain;

namespace Warehouse.Api.Logs.Business
{
    public class LogService : ServiceBase<Log>, ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository, IMapper mapper, IBus bus, IOptions<PollySettings> pollySettings)
            : base(mapper, bus, pollySettings.Value)
        {
            _logRepository = logRepository;
        }


        public async Task<Result<List<LogDto>>> GetAllAsync()
        {
            var logsInDb = await DbPolicy.ExecuteAsync(() => _logRepository.GetRangeAsync(_ => true));
            var logs = Mapper.Map<List<LogDto>>(logsInDb);

            return Result<List<LogDto>>.Success(logs);
        }

        public async Task<Result<List<LogDto>>> GetActualAsync()
        {
            var logsInDb = await DbPolicy.ExecuteAsync(() =>
                _logRepository.GetRangeAsync(l => l.ActionDate >= DateTime.UtcNow.AddDays(-1)));
            var logs = Mapper.Map<List<LogDto>>(logsInDb);

            return Result<List<LogDto>>.Success(logs);
        }

        public async Task<Result<LogDto>> GetAsync(string id)
        {
            var logInDb = await DbPolicy.ExecuteAsync(() => _logRepository.GetAsync(l => l.Id == id));
            CheckForNull(logInDb);
            var log = Mapper.Map<LogDto>(logInDb);

            return Result<LogDto>.Success(log);
        }

        public async Task CreateAsync(LogDto log)
        {
            var logToDb = Mapper.Map<Log>(log);
            await DbPolicy.ExecuteAsync(() => _logRepository.CreateAsync(logToDb));
        }
    }
}