using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Users;

public class FirstNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnFailure_WhenValueIsEmpty(string? value)
    {
        Result<FirstName> result = FirstName.Create(value);

        result.Error.Should().Be(FirstNameErrors.Empty);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenValueIsTooLong()
    {
        Result<FirstName> result = FirstName.Create(
            new string('a', FirstName.MaxLength + 1));

        result.Error.Should().Be(FirstNameErrors.TooLong);
    }

    [Fact]
    public void Create_Should_TrimValue()
    {
        Result<FirstName> result = FirstName.Create(" First ");

        result.Value.Value.Should().Be("First");
    }
}
