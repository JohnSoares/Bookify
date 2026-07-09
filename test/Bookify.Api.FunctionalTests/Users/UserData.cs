namespace Bookify.Api.FunctionalTests.Users;

internal static class UserData
{
    public static readonly RegisterUserRequest RegisterTestUserRequest =
        new("test@test.com", "test", "test", "12345");
}

internal sealed record RegisterUserRequest(string Email, string FirstName, string LastName, string Password);

internal sealed record LogInUserRequest(string Email, string Password);
