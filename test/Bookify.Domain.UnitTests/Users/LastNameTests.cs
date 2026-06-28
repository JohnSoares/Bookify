using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Users;

public class LastNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnFailure_WhenValueIsEmpty(string? value)
    {
        Result<LastName> result = LastName.Create(value);

        result.Error.Should().Be(LastNameErrors.Empty);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenValueIsTooLong()
    {
        Result<LastName> result = LastName.Create(
            new string('a', LastName.MaxLength + 1));

        result.Error.Should().Be(LastNameErrors.TooLong);
    }

    [Fact]
    public void Create_Should_TrimValue()
    {
        Result<LastName> result = LastName.Create(" Last ");

        result.Value.Value.Should().Be("Last");
    }
}
