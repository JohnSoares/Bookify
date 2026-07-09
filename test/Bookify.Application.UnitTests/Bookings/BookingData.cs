using Bookify.Application.UnitTests.Apartments;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;

namespace Bookify.Application.UnitTests.Bookings;

internal static class BookingData
{
    private static readonly DateTime UtcNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    public static Booking CreateReserved()
    {
        DateRange duration = DateRange
            .Create(new DateOnly(2024, 1, 10), new DateOnly(2024, 1, 20))
            .Value;
        Apartment apartment = ApartmentData.Create();

        return Booking.Reserve(apartment, Guid.NewGuid(), duration, UtcNow);
    }

    public static Booking CreateConfirmed()
    {
        Booking booking = CreateReserved();
        booking.Confirm(UtcNow);

        return booking;
    }

    public static Booking CreateCompleted()
    {
        Booking booking = CreateConfirmed();
        booking.Complete(UtcNow);

        return booking;
    }
}
