using Bookify.Api.Extensions;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Users.GetLoggedInUser;
using Bookify.Domain.Abstractions;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Bookify.Api.Endpoints.Users;

internal sealed class GetLoggedInUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/me", async (
            IQueryHandler<GetLoggedInUserQuery, UserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLoggedInUserQuery();

            Result<UserResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.UsersRead)
        .WithTags(Tags.Users);
    }
}
