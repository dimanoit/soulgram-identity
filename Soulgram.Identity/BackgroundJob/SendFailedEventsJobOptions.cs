namespace soulgram.identity.BackgroundJob;

public record SendFailedEventsJobOptions
{
    public string CronExpression { get; init; }
}