using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Users.Commands
{
    public record MakeAdminUserCommand(string UserId) : IRequest<Result<object>>;

    public class MakeAdminUserCommandHandler : IRequestHandler<MakeAdminUserCommand, Result<object>>
    {
        private readonly IUserRepository _userRepository;

        public MakeAdminUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<object>> Handle(MakeAdminUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => u.Id == request.UserId);
            user.CheckForNull();
            if (user.Roles.Contains("Admin"))
            {
                return Result<object>.Success();
            }

            var roles = user.Roles.ToList();
            roles.Add("Admin");
            user.Roles = roles;
            await _userRepository.UpdateAsync(u => u.Id == request.UserId, user);

            return Result<object>.Success();
        }
    }
}