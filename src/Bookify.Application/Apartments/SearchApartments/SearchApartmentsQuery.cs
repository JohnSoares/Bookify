using Bookify.Application.Abstractions.Caching;

namespace Bookify.Application.Apartments.SearchApartments;

public sealed record SearchApartmentsQuery(
    DateOnly StartDate,
    DateOnly EndDate) : ICachedQuery<IReadOnlyList<ApartmentResponse>>
{
    public string CacheKey => $"search-apartments-{StartDate}-{EndDate}";

    public TimeSpan? Expiration => null;
}
