using Bookify.Application.Abstractions.Clock;
using Bookify.Domain.Abstractions;

namespace Bookify.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
