using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Bookings;

public class GetBookingTests : BaseIntegrationTest
{
    private static readonly Guid BookingId = Guid.NewGuid();

    public GetBookingTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetBooking_ShouldReturnFailure_WhenBookingIsNotFound()
    {
        // Arrange
        var query = new GetBookingQuery(BookingId);

        // Act
        Result<BookingResponse> result = await HandleQuery<GetBookingQuery, BookingResponse>(query);

        // Assert
        result.Error.Should().Be(BookingErrors.NotFound(BookingId));
    }

    [Fact]
    public async Task GetBooking_ShouldReturnBooking_WhenBookingExistsForLoggedInUser()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        DateOnly endDate = startDate.AddDays(2);
        Guid bookingId = await ReserveBookingAsync(startDate, endDate);

        var query = new GetBookingQuery(bookingId);

        // Act
        Result<BookingResponse> result = await HandleQuery<GetBookingQuery, BookingResponse>(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(bookingId);
        result.Value.DurationStart.Should().Be(startDate);
        result.Value.DurationEnd.Should().Be(endDate);
    }
}
