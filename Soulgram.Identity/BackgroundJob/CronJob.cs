using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Hosting;
using Timer = System.Timers.Timer;

namespace soulgram.identity.BackgroundJob;

public abstract class CronJob : IHostedService, IDisposable
{
    private readonly CronExpression _cronExpression;
    private Timer _timer;

    protected CronJob(CronExpression cronExpression)
    {
        _cronExpression = cronExpression;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ScheduleNextRun(cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Stop();
        return Task.CompletedTask;
    }

    public abstract Task DoWorkAsync(CancellationToken cancellationToken);

    private void ScheduleNextRun(CancellationToken cancellationToken)
    {
        var nextRun = _cronExpression.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Utc);
        if (!nextRun.HasValue) return;

        var delay = nextRun.Value - DateTime.UtcNow;
        CreateAndRunTimer(cancellationToken, delay);
    }

    private void CreateAndRunTimer(CancellationToken cancellationToken, TimeSpan delay)
    {
        _timer = new Timer(delay.TotalMilliseconds);
        _timer.Elapsed += async (sender, args) =>
        {
            _timer.Dispose();
            _timer = null;

            if (cancellationToken.IsCancellationRequested) return;

            await DoWorkAsync(cancellationToken);
            ScheduleNextRun(cancellationToken);
        };

        _timer.Start();
    }
}