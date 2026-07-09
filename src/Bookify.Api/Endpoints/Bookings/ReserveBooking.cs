using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Domain.Abstractions;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Bookify.Api.Endpoints.Bookings;

internal sealed class ReserveBooking : IEndpoint
{
    internal sealed record Request(Guid ApartmentId, Guid UserId, DateOnly StartDate, DateOnly EndDate);
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("bookings", async (
            Request request,
            ICommandHandler<ReserveBookingCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ReserveBookingCommand(
                request.ApartmentId,
                request.UserId,
                request.StartDate,
                request.EndDate);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Bookings)
        .RequireAuthorization();
    }
}
