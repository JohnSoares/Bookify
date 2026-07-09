using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Bookings;

public class ReserveBookingTests : BaseIntegrationTest
{
    public ReserveBookingTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ReserveBooking_ShouldReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        Apartment apartment = await CreateApartmentAsync();

        var command = new ReserveBookingCommand(apartment.Id, Guid.NewGuid(), startDate, startDate.AddDays(2));

        // Act
        Result<Guid> result = await HandleCommand<ReserveBookingCommand, Guid>(command);

        // Assert
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task ReserveBooking_ShouldReturnFailure_WhenApartmentIsNotFound()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        TestUser user = await CreateUserAsync();

        var command = new ReserveBookingCommand(Guid.NewGuid(), user.Id, startDate, startDate.AddDays(2));

        // Act
        Result<Guid> result = await HandleCommand<ReserveBookingCommand, Guid>(command);

        // Assert
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task ReserveBooking_ShouldReturnSuccess_WhenApartmentIsAvailable()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        TestUser user = await CreateUserAsync();
        Apartment apartment = await CreateApartmentAsync();

        var command = new ReserveBookingCommand(apartment.Id, user.Id, startDate, startDate.AddDays(2));

        // Act
        Result<Guid> result = await HandleCommand<ReserveBookingCommand, Guid>(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ReserveBooking_ShouldReturnFailure_WhenBookingOverlaps()
    {
        // Arrange
        DateOnly startDate = GetNextStartDate();
        TestUser user = await CreateUserAsync();
        Apartment apartment = await CreateApartmentAsync();

        var command = new ReserveBookingCommand(apartment.Id, user.Id, startDate, startDate.AddDays(2));
        Result<Guid> firstResult = await HandleCommand<ReserveBookingCommand, Guid>(command);
        firstResult.IsSuccess.Should().BeTrue();

        // Act
        Result<Guid> result = await HandleCommand<ReserveBookingCommand, Guid>(command);

        // Assert
        result.Error.Should().Be(BookingErrors.Overlap);
    }
}
