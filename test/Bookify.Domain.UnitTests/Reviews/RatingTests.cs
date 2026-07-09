using Bookify.Domain.Reviews;
using Bookify.Domain.Abstractions;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Reviews;

public class RatingTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Create_Should_ReturnSuccess_WhenValueIsInRange(int value)
    {
        // Act
        Result<Rating> result = Rating.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_Should_ReturnFailure_WhenValueIsOutOfRange(int value)
    {
        // Act
        Result<Rating> result = Rating.Create(value);

        // Assert
        result.Error.Should().Be(ReviewErrors.Invalid);
    }
}
