using Bookify.Application.Bookings.CompleteBooking;
using Bookify.Application.Bookings.ConfirmBooking;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Application.Reviews.AddReview;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Reviews;

public class AddReviewTests : BaseIntegrationTest
{
    public AddReviewTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddReview_ShouldReturnFailure_WhenBookingIsNotFound()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var command = new AddReviewCommand(bookingId, 5, "Great stay");

        // Act
        Result result = await HandleCommand(command);

        // Assert
        result.Error.Should().Be(BookingErrors.NotFound(bookingId));
    }

    [Fact]
    public async Task AddReview_ShouldReturnFailure_WhenRatingIsInvalid()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));

        var command = new AddReviewCommand(bookingId, 0, "Great stay");

        // Act
        Result result = await HandleCommand(command);

        // Assert
        result.Error.Should().Be(ReviewErrors.Invalid);
    }

    [Fact]
    public async Task AddReview_ShouldReturnFailure_WhenBookingIsNotCompleted()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));

        var command = new AddReviewCommand(bookingId, 5, "Great stay");

        // Act
        Result result = await HandleCommand(command);

        // Assert
        result.Error.Should().Be(ReviewErrors.NotEligible);
    }

    [Fact]
    public async Task AddReview_ShouldReturnSuccess_WhenBookingIsCompleted()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));

        Result confirmResult = await HandleCommand(new ConfirmBookingCommand(bookingId));
        confirmResult.IsSuccess.Should().BeTrue();

        Result completeResult = await HandleCommand(new CompleteBookingCommand(bookingId));
        completeResult.IsSuccess.Should().BeTrue();

        var command = new AddReviewCommand(bookingId, 5, "Great stay");

        // Act
        Result result = await HandleCommand(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
