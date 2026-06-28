using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public static class NameErrors
{
    public static readonly Error Empty = Error.Problem(
        "Apartment.Name.Empty",
        "The apartment name is required.");

    public static readonly Error TooLong = Error.Problem(
        "Apartment.Name.TooLong",
        $"The apartment name must not exceed {Name.MaxLength} characters.");
}
