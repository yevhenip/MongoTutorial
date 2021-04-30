using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Users.Commands
{
    public record GetUsersPageCommand(int Page, int PageSize) : IRequest<Result<PageDataDto<UserDto>>>;

    public class GetUsersPageCommandHandler : IRequestHandler<GetUsersPageCommand, Result<PageDataDto<UserDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public GetUsersPageCommandHandler(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<Result<PageDataDto<UserDto>>> Handle(GetUsersPageCommand request,
            CancellationToken cancellationToken)
        {
            var usersInDb = await _userRepository.GetPageAsync(request.Page, request.PageSize);
            var count = await _userRepository.GetCountAsync(_ => true);
            var users = _mapper.Map<List<UserDto>>(usersInDb);
            PageDataDto<UserDto> pageData = new(users, count);

            return Result<PageDataDto<UserDto>>.Success(pageData);
        }
    }
}