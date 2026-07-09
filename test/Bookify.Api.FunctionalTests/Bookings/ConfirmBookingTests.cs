using System.Net;
using Bookify.Api.FunctionalTests.Infrastructure;
using Bookify.Application.Bookings.GetBooking;
using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Api.FunctionalTests.Bookings;

[Collection(FunctionalTestCollection.Name)]
public class ConfirmBookingTests : BaseFunctionalTest
{
    public ConfirmBookingTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Confirm_ShouldReturnUnauthorized_WhenAccessTokenIsMissing()
    {
        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(
            $"api/v1/bookings/{Guid.NewGuid()}/confirm",
            content: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Confirm_ShouldReturnNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(
            $"api/v1/bookings/{Guid.NewGuid()}/confirm",
            content: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Confirm_ShouldReturnNoContent_WhenBookingIsReserved()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(
            $"api/v1/bookings/{bookingId}/confirm",
            content: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        BookingResponse booking = await GetBookingAsync(bookingId);
        booking.Status.Should().Be(BookingStatus.Confirmed.ToString());
    }
}
