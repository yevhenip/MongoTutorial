using System.Threading.Tasks;

namespace Warehouse.Core.Interfaces.Messaging.Sender
{
    public interface ISender
    {
        Task SendMessage<T>(T item, string queue);
    }
}