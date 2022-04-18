using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Soulgram.Identity.EventBus;

namespace soulgram.identity.BackgroundJob;

public sealed class SendFailedEventsJob : CronJob
{
    public SendFailedEventsJob(IServiceProvider services) : base(CronExpression.Parse("* * * * *"))
    {
        Services = services;
    }

    public IServiceProvider Services { get; }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Start sending failed evens");

        using (var scope = Services.CreateScope())
        {
            var eventLogService = scope
                .ServiceProvider
                .GetRequiredService<IIntegrationEventLogService>();

            await eventLogService.PublishFailedEvents();
        }

        Console.WriteLine("Finish sending failed evens");
    }
}