using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Apartments;

public class NameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnFailure_WhenValueIsEmpty(string? value)
    {
        Result<Name> result = Name.Create(value);

        result.Error.Should().Be(NameErrors.Empty);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenValueIsTooLong()
    {
        Result<Name> result = Name.Create(new string('a', Name.MaxLength + 1));

        result.Error.Should().Be(NameErrors.TooLong);
    }

    [Fact]
    public void Create_Should_TrimValue()
    {
        Result<Name> result = Name.Create(" Apartment ");

        result.Value.Value.Should().Be("Apartment");
    }
}
