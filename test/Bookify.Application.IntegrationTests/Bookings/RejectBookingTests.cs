using Bookify.Application.Bookings.RejectBooking;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Bookings;

public class RejectBookingTests : BaseIntegrationTest
{
    private static readonly Guid BookingId = Guid.NewGuid();

    public RejectBookingTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task RejectBooking_ShouldReturnFailure_WhenBookingIsNotFound()
    {
        // Arrange
        var command = new RejectBookingCommand(BookingId);

        // Act
        Result result = await HandleCommand(command);

        // Assert
        result.Error.Should().Be(BookingErrors.NotFound(BookingId));
    }

    [Fact]
    public async Task RejectBooking_ShouldReturnSuccess_WhenBookingIsReserved()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));

        var command = new RejectBookingCommand(bookingId);

        // Act
        Result result = await HandleCommand(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
