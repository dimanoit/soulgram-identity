using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Soulgram.Identity.EventBus;

namespace soulgram.identity.BackgroundJob;

public sealed class SendFailedEventsJob : CronJob
{
    public IServiceProvider Services { get; }

    public SendFailedEventsJob(IServiceProvider services) : base(CronExpression.Parse("* * * * *"))
    {
        Services = services;
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        using (var scope = Services.CreateScope())
        {
            var eventLogService = scope
                .ServiceProvider
                .GetRequiredService<IIntegrationEventLogService>();

            await eventLogService.PublishFailedEvents();
        }
    }
}