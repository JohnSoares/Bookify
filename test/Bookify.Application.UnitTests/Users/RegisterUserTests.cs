using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Users.RegisterUser;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;
using FluentAssertions;
using NSubstitute;

namespace Bookify.Application.UnitTests.Users;

public class RegisterUserTests
{
    private static readonly RegisterUserCommand Command = new(
        "test@test.com",
        "First",
        "Last",
        "password");

    private readonly IAuthenticationService _authenticationServiceMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserTests()
    {
        _authenticationServiceMock = Substitute.For<IAuthenticationService>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new RegisterUserCommandHandler(
            _authenticationServiceMock,
            _userRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "invalid-email",
            string.Empty,
            Command.LastName,
            Command.Password);

        // Act
        Result<Guid> result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
        _userRepositoryMock.DidNotReceiveWithAnyArgs().Insert(default!);
        await _unitOfWorkMock.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_Should_RegisterUser_WhenCommandIsValid()
    {
        // Arrange
        const string identityId = "identity-id";

        _authenticationServiceMock
            .RegisterAsync(Arg.Any<User>(), Command.Password, Arg.Any<CancellationToken>())
            .Returns(identityId);

        // Act
        Result<Guid> result = await _handler.Handle(Command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepositoryMock.Received(1).Insert(Arg.Is<User>(
            user =>
                user.Id == result.Value &&
                user.Email.Value == Command.Email &&
                user.FirstName.Value == Command.FirstName &&
                user.LastName.Value == Command.LastName &&
                user.IdentityId == identityId));
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
