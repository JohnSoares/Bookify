using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public static class DescriptionErrors
{
    public static readonly Error Empty = Error.Problem(
        "Apartment.Description.Empty",
        "The apartment description is required.");

    public static readonly Error TooLong = Error.Problem(
        "Apartment.Description.TooLong",
        $"The apartment description must not exceed {Description.MaxLength} characters.");
}
