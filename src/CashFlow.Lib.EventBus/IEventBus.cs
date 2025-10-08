using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashFlow.Lib.EventBus;

public interface IEventBus
{
    Task PublishAsync<T>(T domainEvent) where T : class;
    Task PublishAsync<T>(IEnumerable<T> domainEvents) where T : class;
    Task SubscribeAsync<T>(T domainEvent) where T : class;
}
