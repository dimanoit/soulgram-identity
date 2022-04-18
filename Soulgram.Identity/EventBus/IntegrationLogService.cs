using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Soulgram.Eventbus;
using Soulgram.Eventbus.Interfaces;
using soulgram.identity;
using soulgram.identity.Data;
using Soulgram.Identity.EventBus.Converter;

namespace Soulgram.Identity.EventBus;

public class IntegrationLogService : IIntegrationEventLogService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEventBus _eventBus;

    public IntegrationLogService(IEventBus eventBus, ApplicationDbContext dbContext)
    {
        _eventBus = eventBus;
        _dbContext = dbContext;
    }

    public async Task TryPublish(IntegrationEvent @event)
    {
        try
        {
            await UpdateIntegrationLogEntry(@event.Id, EventStateEnum.InProgress);
            _eventBus.Publish(@event);
            await UpdateIntegrationLogEntry(@event.Id, EventStateEnum.Published);
        }
        catch (Exception e)
        {
            await UpdateIntegrationLogEntry(@event.Id, EventStateEnum.PublishedFailed);
        }
    }

    public async Task PublishFailedEvents()
    {
        var failedEvents = await _dbContext
            .IntegrationEventLogEntries
            .Where(e => e.State == EventStateEnum.PublishedFailed)
            .Select(e => e.ToIntegrationEvent())
            .ToArrayAsync();

        foreach (var @event in failedEvents) await TryPublish(@event);
    }

    private async Task UpdateIntegrationLogEntry(
        Guid eventId,
        EventStateEnum state)
    {
        var logEntry = await _dbContext.IntegrationEventLogEntries
            .SingleAsync(le => le.EventId == eventId);

        logEntry.State = state;
        if (state == EventStateEnum.InProgress) logEntry.TimesSent++;

        await _dbContext.SaveChangesAsync();
    }
}