using System.Net;
using System.Net.Http.Json;
using Bookify.Api.FunctionalTests.Infrastructure;
using Bookify.Application.Bookings.GetBooking;
using FluentAssertions;

namespace Bookify.Api.FunctionalTests.Bookings;

[Collection(FunctionalTestCollection.Name)]
public class GetBookingTests : BaseFunctionalTest
{
    public GetBookingTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenAccessTokenIsMissing()
    {
        // Act
        HttpResponseMessage response = await HttpClient.GetAsync($"api/v1/bookings/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync($"api/v1/bookings/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_ShouldReturnBooking_WhenBookingExists()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        DateOnly endDate = startDate.AddDays(2);
        Guid bookingId = await ReserveBookingAsync(startDate, endDate);

        // Act
        BookingResponse? booking = await HttpClient.GetFromJsonAsync<BookingResponse>($"api/v1/bookings/{bookingId}");

        // Assert
        booking.Should().NotBeNull();
        booking!.Id.Should().Be(bookingId);
        booking.DurationStart.Should().Be(startDate);
        booking.DurationEnd.Should().Be(endDate);
    }
}
