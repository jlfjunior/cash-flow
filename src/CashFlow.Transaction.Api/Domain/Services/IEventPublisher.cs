namespace CashFlow.Transaction.Api.Domain.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(T eventData, string routingKey) where T : class;
}
