using System.Net.Http.Json;
using System.Net.Http.Headers;
using Bookify.Application.Apartments.SearchApartments;
using Bookify.Application.Bookings.GetBooking;
using Bookify.Api.FunctionalTests.Users;
using Bookify.Application.Users.LogInUser;
using Bookify.Application.Users.GetLoggedInUser;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Bookify.Api.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTest
{
    private static int _dateOffset;

    protected readonly HttpClient HttpClient;

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }

    protected async Task AuthenticateAsync()
    {
        string accessToken = await GetAccessToken();

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);
    }

    protected async Task<UserResponse> GetLoggedInUserAsync()
    {
        await AuthenticateAsync();

        UserResponse? user = await HttpClient.GetFromJsonAsync<UserResponse>("api/v1/users/me");

        return user ?? throw new InvalidOperationException("Logged in user endpoint returned an empty response.");
    }

    protected static DateOnly GetNextStartDate() =>
        DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(30 + Interlocked.Increment(ref _dateOffset) * 3);

    protected async Task<ApartmentResponse> GetAvailableApartmentAsync(DateOnly startDate, DateOnly endDate)
    {
        await AuthenticateAsync();

        IReadOnlyList<ApartmentResponse>? apartments = await HttpClient.GetFromJsonAsync<IReadOnlyList<ApartmentResponse>>(
            $"api/v1/apartments?startDate={startDate:O}&endDate={endDate:O}");

        return apartments is { Count: > 0 }
            ? apartments[0]
            : throw new InvalidOperationException("No available apartment was returned by the apartments endpoint.");
    }

    protected async Task<Guid> ReserveBookingAsync(DateOnly startDate, DateOnly endDate)
    {
        UserResponse user = await GetLoggedInUserAsync();
        ApartmentResponse apartment = await GetAvailableApartmentAsync(startDate, endDate);

        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(
            "api/v1/bookings",
            new ReserveBookingRequest(apartment.Id, user.Id, startDate, endDate));

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    protected async Task<BookingResponse> GetBookingAsync(Guid bookingId)
    {
        BookingResponse? booking = await HttpClient.GetFromJsonAsync<BookingResponse>($"api/v1/bookings/{bookingId}");

        return booking ?? throw new InvalidOperationException("Booking endpoint returned an empty response.");
    }

    protected async Task<string> GetAccessToken()
    {
        HttpResponseMessage loginResponse = await HttpClient.PostAsJsonAsync(
            "api/v1/users/login",
            new LogInUserRequest(
                UserData.RegisterTestUserRequest.Email,
                UserData.RegisterTestUserRequest.Password));

        loginResponse.EnsureSuccessStatusCode();

        AccessTokenResponse accessTokenResponse =
            await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>()
            ?? throw new InvalidOperationException("Login did not return an access token.");

        return accessTokenResponse.AccessToken;
    }

    protected sealed record ReserveBookingRequest(
        Guid ApartmentId,
        Guid UserId,
        DateOnly StartDate,
        DateOnly EndDate);
}
