namespace Bookify.Api.Controllers.Reviews;

public sealed record AddReviewRequest
{
    public required Guid BookingId { get; init; }
    public required int Rating { get; init; }
    public string Comment { get; init; }
}
