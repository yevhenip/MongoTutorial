using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Warehouse.Api.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Domain;

namespace Warehouse.Api.Logs.Business
{
    public class LogService : ServiceBase<Log>, ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository, IMapper mapper) : base(mapper)
        {
            _logRepository = logRepository;
        }


        public async Task<Result<List<LogDto>>> GetAllAsync()
        {
            var logsInDb = await _logRepository.GetAllAsync();
            var logs = Mapper.Map<List<LogDto>>(logsInDb);

            return Result<List<LogDto>>.Success(logs);
        }

        public async Task<Result<List<LogDto>>> GetActualAsync()
        {
            var logsInDb = await _logRepository.GetActualAsync();
            var logs = Mapper.Map<List<LogDto>>(logsInDb);

            return Result<List<LogDto>>.Success(logs);
        }

        public async Task<Result<LogDto>> GetAsync(string id)
        {
            var logInDb = await _logRepository.GetAsync(id);
            CheckForNull(logInDb);
            var log = Mapper.Map<LogDto>(logInDb);

            return Result<LogDto>.Success(log);
        }

        public async Task CreateAsync(LogDto log)
        {
            var logToDb = Mapper.Map<Log>(log);
            await _logRepository.CreateAsync(logToDb);
        }
    }
}