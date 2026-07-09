using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Reviews.AddReview;
using Bookify.Domain.Abstractions;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Bookify.Api.Endpoints.Reviews;

internal sealed class AddReview : IEndpoint
{
    internal sealed record Request(Guid BookingId, int Rating, string Comment);
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("reviews", async (
            Request request,
            ICommandHandler<AddReviewCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddReviewCommand(request.BookingId, request.Rating, request.Comment);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Reviews)
        .RequireAuthorization();
    }
}
