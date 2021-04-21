using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Warehouse.Core.Common;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Common
{
    public class ValidateTokenSessionId : JwtBearerEvents
    {
        private readonly IUserRepository _userRepository;

        public ValidateTokenSessionId(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            var userId = context.Principal?.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;
            var user = await _userRepository.GetAsync(u => u.Id == userId);
            var sessionId = context.Principal?.Claims.SingleOrDefault(c => c.Type == "SessionId")?.Value;
            if (user.SessionId != sessionId)
            {
                throw Result<object>.Failure("token", "This is invalid token",
                    HttpStatusCode.BadRequest);
            }
        }
    }
}