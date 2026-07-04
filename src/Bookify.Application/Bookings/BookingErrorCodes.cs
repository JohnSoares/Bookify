namespace Bookify.Application.Bookings;

public static class BookingErrorCodes
{
    public static class ReserveBooking
    {
        public const string MissingUserId = nameof(MissingUserId);
        public const string MissingApartmentId = nameof(MissingApartmentId);
        public const string InvalidDates = nameof(InvalidDates);
    }
}
