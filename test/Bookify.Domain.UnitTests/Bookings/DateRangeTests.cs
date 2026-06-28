using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Bookings;

public class DateRangeTests
{
    [Theory]
    [InlineData(10, 1)]
    [InlineData(1, 1)]
    public void Create_Should_ReturnFailure_WhenRangeIsInvalid(
        int startDay,
        int endDay)
    {
        // Act
        Result<DateRange> result = DateRange.Create(
            new DateOnly(2024, 1, startDay),
            new DateOnly(2024, 1, endDay));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DateRangeErrors.InvalidRange);
    }
}
