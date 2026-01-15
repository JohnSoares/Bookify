namespace Bookify.Api.Controllers.Bookings;

internal sealed record ReserveBookingRequest(
    Guid ApartmentId,
    Guid UserId,
    DateOnly StartDate,
    DateOnly EndDate);
