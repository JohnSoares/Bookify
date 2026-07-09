using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Users.LogInUser;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;
using FluentAssertions;
using NSubstitute;

namespace Bookify.Application.UnitTests.Users;

public class LogInUserTests
{
    private static readonly LogInUserCommand Command = new("test@test.com", "password");

    private readonly IJwtService _jwtServiceMock;
    private readonly LogInUserCommandHandler _handler;

    public LogInUserTests()
    {
        _jwtServiceMock = Substitute.For<IJwtService>();
        _handler = new LogInUserCommandHandler(_jwtServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenCredentialsAreInvalid()
    {
        // Arrange
        _jwtServiceMock
            .GetAccessTokenAsync(Command.Email, Command.Password, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<string>(Error.Problem("Jwt.InvalidCredentials", "Invalid credentials")));

        // Act
        Result<AccessTokenResponse> result = await _handler.Handle(Command, default);

        // Assert
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_Should_ReturnAccessToken_WhenCredentialsAreValid()
    {
        // Arrange
        const string accessToken = "access-token";

        _jwtServiceMock
            .GetAccessTokenAsync(Command.Email, Command.Password, Arg.Any<CancellationToken>())
            .Returns(accessToken);

        // Act
        Result<AccessTokenResponse> result = await _handler.Handle(Command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be(accessToken);
    }
}
