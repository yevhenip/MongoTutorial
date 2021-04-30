using System.Threading;
using System.Threading.Tasks;

namespace Warehouse.Core.Interfaces.Services
{
    public interface ISender
    {
        Task PublishAsync<T>(T item, CancellationToken cancellationToken = default);
    }
}