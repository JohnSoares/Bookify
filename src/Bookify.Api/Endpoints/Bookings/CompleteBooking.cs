using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.CompleteBooking;
using Bookify.Domain.Abstractions;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Bookify.Api.Endpoints.Bookings;

internal sealed class CompleteBooking : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("bookings/{id:guid}/complete", async (
            Guid id,
            ICommandHandler<CompleteBookingCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CompleteBookingCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Bookings)
        .RequireAuthorization();
    }
}
