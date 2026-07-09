using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application.IntegrationTests.Infrastructure;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected Task<Result> HandleCommand<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        ICommandHandler<TCommand> handler = _scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        return handler.Handle(command, cancellationToken);
    }

    protected Task<Result<TResponse>> HandleCommand<TCommand, TResponse>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>
    {
        ICommandHandler<TCommand, TResponse> handler =
            _scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();

        return handler.Handle(command, cancellationToken);
    }

    protected Task<Result<TResponse>> HandleQuery<TQuery, TResponse>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>
    {
        IQueryHandler<TQuery, TResponse> handler =
            _scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();

        return handler.Handle(query, cancellationToken);
    }
}
