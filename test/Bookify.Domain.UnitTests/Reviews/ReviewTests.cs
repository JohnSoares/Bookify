using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews;
using Bookify.Domain.Reviews.Events;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Domain.UnitTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Reviews;

public class ReviewTests : BaseTest
{
    private static readonly DateTime UtcNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_Should_ReturnFailure_WhenBookingIsNotCompleted()
    {
        // Arrange
        Booking booking = CreateReservedBooking();
        Rating rating = Rating.Create(5).Value;
        var comment = new Comment("Great stay");

        // Act
        Result<Review> result = Review.Create(booking, rating, comment, UtcNow);

        // Assert
        result.Error.Should().Be(ReviewErrors.NotEligible);
    }

    [Fact]
    public void Create_Should_SetPropertyValues_WhenBookingIsCompleted()
    {
        // Arrange
        Booking booking = CreateCompletedBooking();
        Rating rating = Rating.Create(5).Value;
        var comment = new Comment("Great stay");

        // Act
        Result<Review> result = Review.Create(booking, rating, comment, UtcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ApartmentId.Should().Be(booking.ApartmentId);
        result.Value.BookingId.Should().Be(booking.Id);
        result.Value.UserId.Should().Be(booking.UserId);
        result.Value.Rating.Should().Be(rating);
        result.Value.Comment.Should().Be(comment);
        result.Value.CreatedOnUtc.Should().Be(UtcNow);
    }

    [Fact]
    public void Create_Should_RaiseReviewCreatedDomainEvent_WhenBookingIsCompleted()
    {
        // Arrange
        Booking booking = CreateCompletedBooking();
        Rating rating = Rating.Create(5).Value;
        var comment = new Comment("Great stay");

        // Act
        Result<Review> result = Review.Create(booking, rating, comment, UtcNow);

        // Assert
        ReviewCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<ReviewCreatedDomainEvent>(result.Value);
        domainEvent.ReviewId.Should().Be(result.Value.Id);
    }

    private static Booking CreateReservedBooking()
    {
        var price = new Money(10.0m, Currency.USD);
        DateRange duration = DateRange
            .Create(new DateOnly(2024, 1, 10), new DateOnly(2024, 1, 20))
            .Value;
        Apartment apartment = ApartmentData.Create(price);

        return Booking.Reserve(apartment, Guid.NewGuid(), duration, UtcNow);
    }

    private static Booking CreateCompletedBooking()
    {
        Booking booking = CreateReservedBooking();
        booking.Confirm(UtcNow);
        booking.Complete(UtcNow);

        return booking;
    }
}
