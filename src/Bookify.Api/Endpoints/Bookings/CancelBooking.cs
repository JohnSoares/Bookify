using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.CancelBooking;
using Bookify.Domain.Abstractions;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Bookify.Api.Endpoints.Bookings;

internal sealed class CancelBooking : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("bookings/{id:guid}/cancel", async (
            Guid id,
            ICommandHandler<CancelBookingCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CancelBookingCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Bookings)
        .RequireAuthorization();
    }
}
