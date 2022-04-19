using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Soulgram.Identity.EventBus;

namespace soulgram.identity.BackgroundJob;

public sealed class SendFailedEventsJob : CronJob
{
    //TODO get cron expression from config
    public SendFailedEventsJob(IServiceProvider services) : base(CronExpression.Parse("* * * * *"))
    {
        Services = services;
    }

    private IServiceProvider Services { get; }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        using var scope = Services.CreateScope();
        var eventLogService = scope
            .ServiceProvider
            .GetRequiredService<IIntegrationEventLogService>();

        await eventLogService.PublishFailedEvents();
    }
}