using Bookify.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Bookify.Domain.Abstractions;
using Serilog.Context;

namespace Bookify.Application.Abstractions.Behaviors;

internal static class LoggingDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(
            TCommand command,
            CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Executing command {CommandName}.", commandName);
            }

            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Command {CommandName} processed successfully.", commandName);
                }
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Command {CommandName} processed with error", commandName);
                }
            }

            return result;
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Executing command {CommandName}.", commandName);
            }

            Result result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Command {CommandName} processed successfully.", commandName);
                }
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Command {CommandName} processed with error", commandName);
                }
            }

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            string queryName = typeof(TQuery).Name;

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Executing query {Query}.", queryName);
            }

            Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Query {Query} processed successfully.", queryName);
                }
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Query {Query} processed with error", queryName);
                }
            }

            return result;
        }
    }
}

