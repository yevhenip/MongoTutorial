using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Users.Commands
{
    public record GetByUserNameCommand(string UserName) : IRequest<Result<UserDto>>;

    public class GetByUserNameCommandHandler : IRequestHandler<GetByUserNameCommand, Result<UserDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public GetByUserNameCommandHandler(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<Result<UserDto>> Handle(GetByUserNameCommand request, CancellationToken cancellationToken)
        {
            var userInDb = await _userRepository.GetAsync(u => u.UserName == request.UserName);
            userInDb.CheckForNull();

            var user = _mapper.Map<UserDto>(userInDb);

            return Result<UserDto>.Success(user);
        }
    }
}