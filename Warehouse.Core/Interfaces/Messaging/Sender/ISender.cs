using System.Threading.Tasks;

namespace Warehouse.Core.Interfaces.Messaging.Sender
{
    public interface ISender
    {
        /// <summary>
        /// Sends data to other microservices
        /// </summary>
        /// <param name="item"></param>
        /// <param name="queue"></param>
        /// <typeparam name="T"></typeparam>
        Task SendMessage<T>(T item, string queue);
    }
}