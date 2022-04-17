using System;
using System.Reflection;
using Newtonsoft.Json;
using Soulgram.Eventbus;
using soulgram.identity;
using Soulgram.Identity.IntegrationEvents;

namespace Soulgram.Identity.EventBus.Converter;

public static class IntegrationEventsConverter
{
    public static IntegrationEventLogEntry ToIntegrationEventLogEntry(this IntegrationEvent @event)
    {
        var integrationEventEntry = new IntegrationEventLogEntry
        {
            EventId = @event.Id,
            Content = @event.ToJson(),
            EventName = @event.GetType().FullName,

            TimesSent = 0,
            State = EventStateEnum.NotPublished,
            CreationTime = DateTime.Now
        };

        return integrationEventEntry;
    }

    public static IntegrationEvent ToIntegrationEvent(this IntegrationEventLogEntry entry)
    {
        var eventType = typeof(Startup)
            .Assembly
            .GetType(entry.EventName);

        if (eventType != null && !eventType.IsSubclassOf(typeof(IntegrationEvent)))
        {
            throw new Exception("It's not a integration event type");
        }
        
        var integrationEvent = JsonConvert.DeserializeObject(entry.Content, eventType) as IntegrationEvent;
        return integrationEvent;
    } 

    private static string ToJson(this IntegrationEvent @event)
    {
        return JsonConvert.SerializeObject(@event);
    }
}