using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Domain;

namespace Warehouse.Api.Auth.Receivers
{
    public class DeletedRefreshTokenHandler : IConsumeAsync<RefreshToken>
    {
        private readonly IAuthService _authService;

        public DeletedRefreshTokenHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task ConsumeAsync(RefreshToken message, CancellationToken cancellationToken = new())
        {
            await _authService.DeleteTokenAsync(message);
        }
    }
}