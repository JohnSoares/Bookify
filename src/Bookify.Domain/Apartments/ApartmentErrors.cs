using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public static class ApartmentErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        "Apartment.NotFound",
        $"The apartment with the Id = '{id}' was not found");
}
