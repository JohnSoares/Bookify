using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        "User.NotFound",
        $"The user with the Id = '{id}' was not found.");

    public static readonly Error InvalidCredentials = Error.Failure(
        "User.InvalidCredentials",
        "The provided credentials were invalid.");
}
