using System.Threading.Tasks;
using Soulgram.Eventbus;

namespace Soulgram.Identity.EventBus;

public interface IIntegrationEventLogService
{
    Task TryPublish(IntegrationEvent @event);

    Task PublishFailedEvents();
}