using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Logs.Commands
{
    public record GetLogsCommand(Expression<Func<Log, bool>> Predicate) : IRequest<Result<List<LogDto>>>;

    public class GetLogsCommandHandler : IRequestHandler<GetLogsCommand, Result<List<LogDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ILogRepository _logRepository;

        public GetLogsCommandHandler(IMapper mapper, ILogRepository logRepository)
        {
            _mapper = mapper;
            _logRepository = logRepository;
        }

        public async Task<Result<List<LogDto>>> Handle(GetLogsCommand request, CancellationToken cancellationToken)
        {
            var logsInDb = await _logRepository.GetRangeAsync(request.Predicate);
            var logs = _mapper.Map<List<LogDto>>(logsInDb);

            return Result<List<LogDto>>.Success(logs);
        }
    }
}