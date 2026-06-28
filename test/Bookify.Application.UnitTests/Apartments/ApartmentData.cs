using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Application.UnitTests.Apartments;

internal static class ApartmentData
{
    public static Apartment Create() => new(
        Guid.NewGuid(),
        Name.Create("Test apartment").Value,
        Description.Create("Test description").Value,
        Address.Create("Country", "State", "ZipCode", "City", "Street").Value,
        new Money(100.0m, Currency.USD),
        Money.Zero(),
        []);
}
