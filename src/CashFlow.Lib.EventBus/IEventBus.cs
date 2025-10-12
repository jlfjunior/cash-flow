using System.Threading.Tasks;

namespace CashFlow.Lib.EventBus;

public interface IEventBus
{
    Task PublishAsync<T>(T @event) where T : class;
}
