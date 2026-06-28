using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;

public static class LastNameErrors
{
    public static readonly Error Empty = Error.Problem(
        "User.LastName.Empty",
        "The last name is required.");

    public static readonly Error TooLong = Error.Problem(
        "User.LastName.TooLong",
        $"The last name must not exceed {LastName.MaxLength} characters.");
}
