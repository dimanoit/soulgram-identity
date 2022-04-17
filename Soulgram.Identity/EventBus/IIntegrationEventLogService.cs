using System.Collections.Generic;
using System.Threading.Tasks;
using Soulgram.Eventbus;

namespace Soulgram.Identity.EventBus;

public interface IIntegrationEventLogService
{
    Task TryPublish(IntegrationEvent @event);

    Task<IEnumerable<IntegrationEvent>> GetFailedEvents();
}