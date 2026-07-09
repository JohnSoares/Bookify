using System.Net;
using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Api.FunctionalTests.Bookings;

[Collection(FunctionalTestCollection.Name)]
public class CompleteBookingTests : BaseFunctionalTest
{
    public CompleteBookingTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Complete_ShouldReturnUnauthorized_WhenAccessTokenIsMissing()
    {
        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(
            $"api/v1/bookings/{Guid.NewGuid()}/complete",
            content: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Complete_ShouldReturnBadRequest_WhenBookingIsNotConfirmed()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(
            $"api/v1/bookings/{bookingId}/complete",
            content: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Complete_ShouldReturnNoContent_WhenBookingIsConfirmed()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));
        HttpResponseMessage confirmResponse = await HttpClient.PostAsync(
            $"api/v1/bookings/{bookingId}/confirm",
            content: null);
        confirmResponse.EnsureSuccessStatusCode();

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(
            $"api/v1/bookings/{bookingId}/complete",
            content: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
