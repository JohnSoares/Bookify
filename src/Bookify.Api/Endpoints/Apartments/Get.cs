using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Apartments.SearchApartments;
using Bookify.Domain.Abstractions;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Bookify.Api.Endpoints.Apartments;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("apartments", async (
            DateOnly startDate,
            DateOnly endDate,
            IQueryHandler<SearchApartmentsQuery, IReadOnlyList<ApartmentResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new SearchApartmentsQuery(startDate, endDate);

            Result<IReadOnlyList<ApartmentResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Apartments)
        .RequireAuthorization();
    }
}
