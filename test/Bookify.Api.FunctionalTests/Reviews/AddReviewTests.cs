using System.Net;
using System.Net.Http.Json;
using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Api.FunctionalTests.Reviews;

[Collection(FunctionalTestCollection.Name)]
public class AddReviewTests : BaseFunctionalTest
{
    public AddReviewTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Add_ShouldReturnUnauthorized_WhenAccessTokenIsMissing()
    {
        // Arrange
        var request = new AddReviewRequest(Guid.NewGuid(), 5, "Great stay");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/reviews", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Add_ShouldReturnNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        await AuthenticateAsync();
        var request = new AddReviewRequest(Guid.NewGuid(), 5, "Great stay");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/reviews", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_ShouldReturnBadRequest_WhenBookingIsNotCompleted()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));
        var request = new AddReviewRequest(bookingId, 5, "Great stay");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/reviews", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Add_ShouldReturnNoContent_WhenBookingIsCompleted()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Guid bookingId = await ReserveBookingAsync(startDate, startDate.AddDays(2));
        HttpResponseMessage confirmResponse = await HttpClient.PostAsync(
            $"api/v1/bookings/{bookingId}/confirm",
            content: null);
        confirmResponse.EnsureSuccessStatusCode();

        HttpResponseMessage completeResponse = await HttpClient.PostAsync(
            $"api/v1/bookings/{bookingId}/complete",
            content: null);
        completeResponse.EnsureSuccessStatusCode();

        var request = new AddReviewRequest(bookingId, 5, "Great stay");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/reviews", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private sealed record AddReviewRequest(Guid BookingId, int Rating, string Comment);
}
