using Bookify.Domain.Shared;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Shared;

public class MoneyTests
{
    [Fact]
    public void Add_Should_ReturnMoneyWithSummedAmount_WhenCurrenciesAreEqual()
    {
        // Arrange
        var first = new Money(10.0m, Currency.USD);
        var second = new Money(25.5m, Currency.USD);

        // Act
        Money result = first + second;

        // Assert
        result.Should().Be(new Money(35.5m, Currency.USD));
    }

    [Fact]
    public void Add_Should_ThrowInvalidOperationException_WhenCurrenciesAreDifferent()
    {
        // Arrange
        var first = new Money(10.0m, Currency.USD);
        var second = new Money(25.5m, Currency.EUR);

        // Act
        Action act = () => _ = first + second;

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void IsZero_Should_ReturnTrue_WhenAmountIsZeroForSameCurrency()
    {
        // Arrange
        var money = new Money(0, Currency.USD);

        // Act
        bool result = money.IsZero();

        // Assert
        result.Should().BeTrue();
    }
}
