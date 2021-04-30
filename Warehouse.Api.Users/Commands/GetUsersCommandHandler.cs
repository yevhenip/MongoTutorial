using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Users.Commands
{
    public record GetUsersCommand(Expression<Func<User, bool>> Predicate) : IRequest<Result<List<UserDto>>>;

    public class GetUsersCommandHandler : IRequestHandler<GetUsersCommand, Result<List<UserDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public GetUsersCommandHandler(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<Result<List<UserDto>>> Handle(GetUsersCommand request, CancellationToken cancellationToken)
        {
            var usersFromDb = await _userRepository.GetRangeAsync(_ => true);
            var users = _mapper.Map<List<UserDto>>(usersFromDb);

            return Result<List<UserDto>>.Success(users);
        }
    }
}