using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Bookings;

public static class BookingErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        "Booking.NotFound",
        $"The booking with the Id = '{id}' was not found");

    public static readonly Error Overlap = Error.Problem(
        "Booking.Overlap",
        "The current booking is overlapping with an existing one.");

    public static readonly Error NotReserved = Error.Problem(
        "Booking.NotReserved",
        "The booking is not pending.");

    public static readonly Error NotConfirmed = Error.Problem(
        "Booking.NotConfirmed",
        "The booking is not confirmed.");

    public static readonly Error AlreadyStarted = Error.Problem(
        "Booking.AlreadyStarted",
        "The booking has already started.");
}
