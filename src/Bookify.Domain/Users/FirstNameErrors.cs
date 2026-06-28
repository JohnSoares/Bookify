using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;

public static class FirstNameErrors
{
    public static readonly Error Empty = Error.Problem(
        "User.FirstName.Empty",
        "The first name is required.");

    public static readonly Error TooLong = Error.Problem(
        "User.FirstName.TooLong",
        $"The first name must not exceed {FirstName.MaxLength} characters.");
}
