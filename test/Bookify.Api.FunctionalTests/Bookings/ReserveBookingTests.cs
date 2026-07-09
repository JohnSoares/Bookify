using System.Net;
using System.Net.Http.Json;
using Bookify.Api.FunctionalTests.Infrastructure;
using Bookify.Application.Apartments.SearchApartments;
using Bookify.Application.Users.GetLoggedInUser;
using FluentAssertions;

namespace Bookify.Api.FunctionalTests.Bookings;

[Collection(FunctionalTestCollection.Name)]
public class ReserveBookingTests : BaseFunctionalTest
{
    public ReserveBookingTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Reserve_ShouldReturnUnauthorized_WhenAccessTokenIsMissing()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        DateOnly endDate = startDate.AddDays(2);

        var request = new ReserveBookingRequest(Guid.NewGuid(), Guid.NewGuid(), startDate, endDate);

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/bookings", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Reserve_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        DateOnly endDate = startDate.AddDays(-1);
        await AuthenticateAsync();

        var request = new ReserveBookingRequest(Guid.Empty, Guid.Empty, startDate, endDate);

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/bookings", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Reserve_ShouldReturnOk_WhenApartmentIsAvailable()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        DateOnly endDate = startDate.AddDays(2);
        UserResponse user = await GetLoggedInUserAsync();
        ApartmentResponse apartment = await GetAvailableApartmentAsync(startDate, endDate);

        var request = new ReserveBookingRequest(apartment.Id, user.Id, startDate, endDate);

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/bookings", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Guid bookingId = await response.Content.ReadFromJsonAsync<Guid>();
        bookingId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Reserve_ShouldReturnBadRequest_WhenBookingOverlaps()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        DateOnly endDate = startDate.AddDays(2);
        UserResponse user = await GetLoggedInUserAsync();
        ApartmentResponse apartment = await GetAvailableApartmentAsync(startDate, endDate);

        var request = new ReserveBookingRequest(apartment.Id, user.Id, startDate, endDate);
        HttpResponseMessage firstResponse = await HttpClient.PostAsJsonAsync("api/v1/bookings", request);
        firstResponse.EnsureSuccessStatusCode();

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/bookings", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
