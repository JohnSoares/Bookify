namespace Bookify.Api.Controllers.Bookings;

public sealed record ReserveBookingRequest
{
    public required Guid ApartmentId { get; init; }
    public required Guid UserId { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
}
