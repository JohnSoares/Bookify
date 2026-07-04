using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews;

namespace Bookify.Application.Reviews.AddReview;

internal sealed class AddReviewCommandHandler(
        IBookingRepository bookingRepository,
        IReviewRepository reviewRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider) : ICommandHandler<AddReviewCommand>
{

    public async Task<Result> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        Booking? booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);

        if (booking is null)
        {
            return Result.Failure(BookingErrors.NotFound(request.BookingId));
        }

        Result<Rating> ratingResult = Rating.Create(request.Rating);

        if (ratingResult.IsFailure)
        {
            return Result.Failure(ratingResult.Error);
        }

        Result<Review> reviewResult = Review.Create(
            booking,
            ratingResult.Value,
            new Comment(request.Comment),
            dateTimeProvider.UtcNow);

        if (reviewResult.IsFailure)
        {
            return Result.Failure(reviewResult.Error);
        }

        reviewRepository.Insert(reviewResult.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
