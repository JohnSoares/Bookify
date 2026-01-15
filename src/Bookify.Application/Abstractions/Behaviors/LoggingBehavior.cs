using MediatR;
using Bookify.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly ILogger<TRequest> _logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        string requestName = request.GetType().Name;

        try
        {
            _logger.LogInformation("Executing command {Command}.", requestName);

            TResponse response = await next(cancellationToken);

            _logger.LogInformation("Command {Command} processed successfully.", requestName);

            return response;
        }
        catch(Exception exception)
        {
            _logger.LogError(exception, "Command {Command} processing failed.", requestName);
            throw;
        }
    }
}
