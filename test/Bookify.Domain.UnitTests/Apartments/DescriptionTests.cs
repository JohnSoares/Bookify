using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Apartments;

public class DescriptionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnFailure_WhenValueIsEmpty(string? value)
    {
        Result<Description> result = Description.Create(value);

        result.Error.Should().Be(DescriptionErrors.Empty);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenValueIsTooLong()
    {
        Result<Description> result = Description.Create(
            new string('a', Description.MaxLength + 1));

        result.Error.Should().Be(DescriptionErrors.TooLong);
    }

    [Fact]
    public void Create_Should_TrimValue()
    {
        Result<Description> result = Description.Create(" Description ");

        result.Value.Value.Should().Be("Description");
    }
}
