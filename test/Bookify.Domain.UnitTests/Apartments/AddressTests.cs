using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Apartments;

public class AddressTests
{
    [Theory]
    [InlineData(null, "State", "ZipCode", "City", "Street")]
    [InlineData("Country", "", "ZipCode", "City", "Street")]
    [InlineData("Country", "State", " ", "City", "Street")]
    [InlineData("Country", "State", "ZipCode", null, "Street")]
    [InlineData("Country", "State", "ZipCode", "City", "")]
    public void Create_Should_ReturnFailure_WhenAnyFieldIsMissing(
        string? country,
        string? state,
        string? zipCode,
        string? city,
        string? street)
    {
        // Act
        Result<Address> result = Address.Create(
            country,
            state,
            zipCode,
            city,
            street);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AddressErrors.RequiredFields);
    }

    [Fact]
    public void Create_Should_TrimAllFields()
    {
        // Act
        Result<Address> result = Address.Create(
            " Country ",
            " State ",
            " ZipCode ",
            " City ",
            " Street ");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Country.Should().Be("Country");
        result.Value.State.Should().Be("State");
        result.Value.ZipCode.Should().Be("ZipCode");
        result.Value.City.Should().Be("City");
        result.Value.Street.Should().Be("Street");
    }
}
