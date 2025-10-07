using CashFlow.Transaction.Api.Domain.Events;

namespace CashFlow.Transaction.Api.Infrastructure.EventBus;

public interface IEventBus
{
    Task PublishAsync(IDomainEvent domainEvent);
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents);
}
