using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Application.Users.GetLoggedInUser;
using Bookify.Domain.Abstractions;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Users;

public class GetLoggedInUserTests : BaseIntegrationTest
{
    public GetLoggedInUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetLoggedInUser_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        TestUser user = await CreateUserAsync();

        var query = new GetLoggedInUserQuery();

        // Act
        Result<UserResponse> result = await HandleQuery<GetLoggedInUserQuery, UserResponse>(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be(user.Email);
    }
}
