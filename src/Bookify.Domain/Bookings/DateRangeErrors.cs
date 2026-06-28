using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Bookings;

public static class DateRangeErrors
{
    public static readonly Error InvalidRange = Error.Problem(
        "DateRange.InvalidRange",
        "The end date must be greater than the start date.");
}
