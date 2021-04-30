using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Logs.Commands
{
    public record CreateLogCommand(LogDto Log) : IRequest;

    public class CreateLogCommandHandler : IRequestHandler<CreateLogCommand>
    {
        private readonly IMapper _mapper;
        private readonly ILogRepository _logRepository;

        public CreateLogCommandHandler(IMapper mapper, ILogRepository logRepository)
        {
            _mapper = mapper;
            _logRepository = logRepository;
        }

        public async Task<Unit> Handle(CreateLogCommand request, CancellationToken cancellationToken)
        {
            var logToDb = _mapper.Map<Log>(request.Log);
            await _logRepository.CreateAsync(logToDb);
            return Unit.Value;
        }
    }
}