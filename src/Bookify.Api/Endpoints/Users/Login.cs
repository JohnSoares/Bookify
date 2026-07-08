using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Users.LogInUser;
using Bookify.Domain.Abstractions;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Bookify.Api.Endpoints.Users;

internal sealed class Login : IEndpoint
{
    internal sealed record Request(string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login", async (
            Request request,
            ICommandHandler<LogInUserCommand, AccessTokenResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LogInUserCommand(request.Email, request.Password);

            Result<AccessTokenResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
