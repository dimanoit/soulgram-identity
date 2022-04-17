using System.Threading.Tasks;
using Soulgram.Identity.EventBus;

namespace soulgram.identity.BackgroundJob;

public class SendFailedEventsJob : CronJob
{ 
    private readonly IIntegrationEventLogService _eventLogService;

    public SendFailedEventsJob(IIntegrationEventLogService eventLogService)
    {
        _eventLogService = eventLogService;
    }

    public async Task SendFailedEvents()
    {
    }
}