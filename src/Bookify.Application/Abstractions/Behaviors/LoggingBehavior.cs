using MediatR;
using Bookify.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Bookify.Domain.Abstractions;
using Serilog.Context;

namespace Bookify.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        try
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Executing request {RequestName}.", requestName);
            }

            TResponse response = await next(cancellationToken);

            if (response.IsSuccess)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Request {RequestName} processed successfully.", requestName);
                }
            }
            else
            {
                using (LogContext.PushProperty("Error", response.Error, true))
                {
                    _logger.LogError("Request {RequestName} processed with error", requestName);
                }
            }

            return response;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Request {RequestName} processing failed.", requestName);
            throw;
        }
    }
}