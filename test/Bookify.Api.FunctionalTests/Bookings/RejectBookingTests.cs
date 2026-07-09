using System.Net;
using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Api.FunctionalTests.Bookings;

[Collection(FunctionalTestCollection.Name)]
public class RejectBookingTests : BaseFunctionalTest
{
    public RejectBookingTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Reject_ShouldReturnUnauthorized_WhenAccessTokenIsMissing()
    {
        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(
            $"api/v1/bookings/{Guid.NewGuid()}/reject",
            content: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Reject_ShouldReturnNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(
            $"api/v1/bookings/{Guid.NewGuid()}/reject",
            content: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Reject_ShouldReturnNoContent_WhenBookingIsReserved()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(
            $"api/v1/bookings/{bookingId}/reject",
            content: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
