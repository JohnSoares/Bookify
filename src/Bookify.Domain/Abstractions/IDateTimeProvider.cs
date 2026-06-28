namespace Bookify.Domain.Abstractions;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}
