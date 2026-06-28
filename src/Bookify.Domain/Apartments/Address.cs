using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public sealed record Address
{
    private Address(
        string country,
        string state,
        string zipCode,
        string city,
        string street)
    {
        Country = country;
        State = state;
        ZipCode = zipCode;
        City = city;
        Street = street;
    }

    public string Country { get; }

    public string State { get; }

    public string ZipCode { get; }

    public string City { get; }

    public string Street { get; }

    public static Result<Address> Create(
        string? country,
        string? state,
        string? zipCode,
        string? city,
        string? street)
    {
        if (string.IsNullOrWhiteSpace(country) ||
            string.IsNullOrWhiteSpace(state) ||
            string.IsNullOrWhiteSpace(zipCode) ||
            string.IsNullOrWhiteSpace(city) ||
            string.IsNullOrWhiteSpace(street))
        {
            return Result.Failure<Address>(AddressErrors.RequiredFields);
        }

        return new Address(
            country.Trim(),
            state.Trim(),
            zipCode.Trim(),
            city.Trim(),
            street.Trim());
    }
}
