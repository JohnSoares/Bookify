using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.GetBooking;
using Bookify.Domain.Abstractions;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Bookify.Api.Endpoints.Bookings;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("bookings/{id:guid}", async (
            Guid id,
            IQueryHandler<GetBookingQuery, BookingResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookingQuery(id);

            Result<BookingResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Bookings)
        .RequireAuthorization();
    }
}
