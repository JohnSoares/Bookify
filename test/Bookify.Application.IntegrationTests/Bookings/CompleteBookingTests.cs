using Bookify.Application.Bookings.CompleteBooking;
using Bookify.Application.Bookings.ConfirmBooking;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Bookings;

public class CompleteBookingTests : BaseIntegrationTest
{
    public CompleteBookingTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CompleteBooking_ShouldReturnFailure_WhenBookingIsNotConfirmed()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));

        var command = new CompleteBookingCommand(bookingId);

        // Act
        Result result = await HandleCommand(command);

        // Assert
        result.Error.Should().Be(BookingErrors.NotConfirmed);
    }

    [Fact]
    public async Task CompleteBooking_ShouldReturnSuccess_WhenBookingIsConfirmed()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));

        Result confirmResult = await HandleCommand(new ConfirmBookingCommand(bookingId));
        confirmResult.IsSuccess.Should().BeTrue();

        var command = new CompleteBookingCommand(bookingId);

        // Act
        Result result = await HandleCommand(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
