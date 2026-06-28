using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Reviews;

public static class ReviewErrors
{
    public static readonly Error NotEligible = Error.Problem(
        "Review.NotEligible",
        "The review is not eligible because the booking is not yet completed");

    public static readonly Error Invalid = Error.Failure(
        "Rating.Invalid", 
        "The rating is invalid");
}
