namespace Bookify.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    public int SchedulePollingInterval { get; init; } = 10;

    public int BatchSize { get; init; } = 10;
}
