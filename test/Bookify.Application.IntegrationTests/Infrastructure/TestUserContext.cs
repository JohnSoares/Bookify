using Bookify.Application.Abstractions.Authentication;

namespace Bookify.Application.IntegrationTests.Infrastructure;

public sealed class TestUserContext : IUserContext
{
    private UserState? _currentUser;

    public Guid UserId =>
        _currentUser?.UserId ??
        throw new InvalidOperationException("Test user context is unavailable");

    public string IdentityId =>
        _currentUser?.IdentityId ??
        throw new InvalidOperationException("Test user context is unavailable");

    public void SetUser(Guid userId, string identityId) =>
        _currentUser = new UserState(userId, identityId);

    private sealed record UserState(Guid UserId, string IdentityId);
}
