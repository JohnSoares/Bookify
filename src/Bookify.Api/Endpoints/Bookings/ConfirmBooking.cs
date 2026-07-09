using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.ConfirmBooking;
using Bookify.Domain.Abstractions;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Bookify.Api.Endpoints.Bookings;

internal sealed class ConfirmBooking : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("bookings/{id:guid}/confirm", async (
            Guid id,
            ICommandHandler<ConfirmBookingCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ConfirmBookingCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Bookings)
        .RequireAuthorization();
    }
}
