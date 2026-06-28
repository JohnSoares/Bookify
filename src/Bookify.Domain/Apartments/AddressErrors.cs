using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public static class AddressErrors
{
    public static readonly Error RequiredFields = Error.Problem(
        "Address.RequiredFields",
        "All address fields are required.");
}
