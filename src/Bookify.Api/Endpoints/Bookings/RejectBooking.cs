using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.RejectBooking;
using Bookify.Domain.Abstractions;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Bookify.Api.Endpoints.Bookings;

internal sealed class RejectBooking : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("bookings/{id:guid}/reject", async (
            Guid id,
            ICommandHandler<RejectBookingCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RejectBookingCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Bookings)
        .RequireAuthorization();
    }
}
