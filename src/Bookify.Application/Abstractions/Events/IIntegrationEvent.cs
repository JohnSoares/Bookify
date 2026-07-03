namespace Bookify.Application.Abstractions.Events;

public interface IIntegrationEvent
{
    Guid Id { get; init; }
}
