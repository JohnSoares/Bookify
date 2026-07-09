using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application.IntegrationTests.Infrastructure;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private static int _dateOffset;

    private readonly IServiceScope _scope;
    private readonly TestUserContext _userContext;

    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _userContext = _scope.ServiceProvider.GetRequiredService<TestUserContext>();
    }

    protected static DateOnly GetNextStartDate() =>
        DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(30 + Interlocked.Increment(ref _dateOffset) * 3);

    protected async Task<TestUser> CreateUserAsync()
    {
        var user = new TestUser(
            Guid.NewGuid(),
            "Test",
            "User",
            $"{Guid.NewGuid():N}@test.com",
            Guid.NewGuid().ToString());

        await DbContext.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO users (id, first_name, last_name, email, identity_id)
            VALUES ({user.Id}, {user.FirstName}, {user.LastName}, {user.Email}, {user.IdentityId})
            """);

        _userContext.SetUser(user.Id, user.IdentityId);

        return user;
    }

    protected async Task<Apartment> CreateApartmentAsync()
    {
        var apartment = new Apartment(
            Guid.NewGuid(),
            Name.Create("Test apartment").Value,
            Description.Create("Test apartment description").Value,
            Address.Create("Brazil", "SP", "01000-000", "Sao Paulo", "Test Street").Value,
            new Money(100, Currency.USD),
            new Money(25, Currency.USD),
            [Amenity.Parking, Amenity.Wifi]);

        DbContext.Set<Apartment>().Add(apartment);
        await DbContext.SaveChangesAsync();

        return apartment;
    }

    protected async Task<Guid> ReserveBookingAsync(DateOnly startDate, DateOnly endDate)
    {
        TestUser user = await CreateUserAsync();
        Apartment apartment = await CreateApartmentAsync();

        var command = new ReserveBookingCommand(apartment.Id, user.Id, startDate, endDate);

        Result<Guid> result = await HandleCommand<ReserveBookingCommand, Guid>(command);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to reserve booking. Error: {result.Error.Code}");
        }

        return result.Value;
    }

    protected sealed record TestUser(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string IdentityId);

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
