using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Logs.Commands
{
    public record GetLogCommand(string Id) : IRequest<Result<LogDto>>;
    
    public class GetLogCommandHandler : IRequestHandler<GetLogCommand, Result<LogDto>>
    {
        private readonly IMapper _mapper;
        private readonly ILogRepository _logRepository;

        public GetLogCommandHandler(IMapper mapper, ILogRepository logRepository)
        {
            _mapper = mapper;
            _logRepository = logRepository;
        }
        
        public async Task<Result<LogDto>> Handle(GetLogCommand request, CancellationToken cancellationToken)
        {
            var logInDb = await _logRepository.GetAsync(l => l.Id == request.Id);
            logInDb.CheckForNull();
            var log = _mapper.Map<LogDto>(logInDb);

            return Result<LogDto>.Success(log);
        }
    }
}