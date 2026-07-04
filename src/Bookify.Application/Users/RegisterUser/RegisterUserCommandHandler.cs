using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;

namespace Bookify.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler(
        IAuthenticationService authenticationService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork) : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        Result<FirstName> firstNameResult = FirstName.Create(request.FirstName);
        Result<LastName> lastNameResult = LastName.Create(request.LastName);
        Result<Email> emailResult = Email.Create(request.Email);

        Result[] validationResults =
        [
            firstNameResult,
            lastNameResult,
            emailResult
        ];

        if (validationResults.Any(result => result.IsFailure))
        {
            return Result.Failure<Guid>(
                ValidationError.FromResults(validationResults));
        }

        var user = User.Create(
            firstNameResult.Value,
            lastNameResult.Value,
            emailResult.Value);

        string identityId = await authenticationService.RegisterAsync(
            user,
            request.Password,
            cancellationToken);

        user.SetIdentityId(identityId);

        userRepository.Insert(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
