using Bookify.Domain.Users;

namespace Bookify.Application.UnitTests.Users;

internal static class UserData
{
    public static User Create() => User.Create(FirstName, LastName, Email);

    public static readonly FirstName FirstName = FirstName.Create("First").Value;
    public static readonly LastName LastName = LastName.Create("Last").Value;
    public static readonly Email Email = Email.Create("test@test.com").Value;
}
