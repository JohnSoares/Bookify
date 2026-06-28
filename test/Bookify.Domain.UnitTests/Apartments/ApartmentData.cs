using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Domain.UnitTests.Apartments;

internal static class ApartmentData
{
    public static Apartment Create(Money price, Money? cleaningFee = null) => new(
        Guid.NewGuid(),
        Name.Create("Test apartment").Value,
        Description.Create("Test description").Value,
        Address.Create("Country", "State", "ZipCode", "City", "Street").Value,
        price,
        cleaningFee ?? Money.Zero(),
        []);
}
