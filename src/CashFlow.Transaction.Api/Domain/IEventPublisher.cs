using CashFlow.Transaction.Api.Domain.Events;

namespace CashFlow.Transaction.Api.Domain;

public interface IEventPublisher
{
    Task PublishAsync<T>(T eventData, string routingKey) where T : class;
}
