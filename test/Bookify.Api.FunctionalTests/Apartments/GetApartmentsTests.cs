using System.Net;
using System.Net.Http.Json;
using Bookify.Api.FunctionalTests.Infrastructure;
using Bookify.Application.Apartments.SearchApartments;
using FluentAssertions;

namespace Bookify.Api.FunctionalTests.Apartments;

[Collection(FunctionalTestCollection.Name)]
public class GetApartmentsTests : BaseFunctionalTest
{
    public GetApartmentsTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenAccessTokenIsMissing()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        DateOnly endDate = startDate.AddDays(2);

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(
            $"api/v1/apartments?startDate={startDate:O}&endDate={endDate:O}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_ShouldReturnApartments_WhenAccessTokenIsNotMissing()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        DateOnly endDate = startDate.AddDays(2);
        await AuthenticateAsync();

        // Act
        IReadOnlyList<ApartmentResponse>? apartments =
            await HttpClient.GetFromJsonAsync<IReadOnlyList<ApartmentResponse>>(
                $"api/v1/apartments?startDate={startDate:O}&endDate={endDate:O}");

        // Assert
        apartments.Should().NotBeNull();
        apartments.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Get_ShouldReturnEmptyList_WhenDateRangeIsInvalid()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        DateOnly endDate = startDate.AddDays(-1);
        await AuthenticateAsync();

        // Act
        IReadOnlyList<ApartmentResponse>? apartments =
            await HttpClient.GetFromJsonAsync<IReadOnlyList<ApartmentResponse>>(
                $"api/v1/apartments?startDate={startDate:O}&endDate={endDate:O}");

        // Assert
        apartments.Should().NotBeNull();
        apartments.Should().BeEmpty();
    }
}
