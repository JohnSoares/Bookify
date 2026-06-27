using Bookify.Api.Controllers.Users;

namespace Bookify.Api.FunctionalTests.Users;

internal static class UserData
{
    public static readonly RegisterUserRequest RegisterTestUserRequest =
        new("test@test.com", "test", "test", "12345");
}
