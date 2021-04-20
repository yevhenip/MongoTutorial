using System.Reflection;

namespace Warehouse.Api.Settings
{
    public class EventSubscriptionSettings
    {
        public Assembly EventHandlersAssemblies { get; set; }
    }
}