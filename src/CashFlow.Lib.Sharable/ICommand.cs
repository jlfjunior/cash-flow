using System.Threading;
using System.Threading.Tasks;

namespace CashFlow.Lib.Sharable;

public interface ICommand<T>
{
    Task ExecuteAsync(T request, CancellationToken token = default);
}

public interface ICommand<T, R>
{
    Task<R> ExecuteAsync(T request, CancellationToken token = default);
}