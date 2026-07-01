using Bookify.Application.Abstractions.Caching;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Abstractions.Behaviors;

internal static class QueryCachingDecorator
{
    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ICacheService cacheService,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(
            TQuery query,
            CancellationToken cancellationToken)
        {
            if (query is not ICachedQuery cachedQuery)
            {
                return await innerHandler.Handle(query, cancellationToken);
            }

            Result<TResponse>? cachedResult =
                await cacheService.GetAsync<Result<TResponse>>(
                    cachedQuery.CacheKey,
                    cancellationToken);

            string queryName = typeof(TQuery).Name;

            if (cachedResult is not null)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Cache hit for {Query}", queryName);
                }

                return cachedResult;
            }

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Cache miss for {Query}", queryName);
            }

            Result<TResponse> result = await innerHandler.Handle(
                query,
                cancellationToken);

            if (result.IsSuccess)
            {
                await cacheService.SetAsync(
                    cachedQuery.CacheKey,
                    result,
                    cachedQuery.Expiration,
                    cancellationToken);
            }

            return result;
        }
    }
}
