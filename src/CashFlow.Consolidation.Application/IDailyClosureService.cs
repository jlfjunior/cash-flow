using CashFlow.Consolidation.Domain.Entities;

namespace CashFlow.Consolidation.Application;

public interface IDailyClosureService
{
    Task<DailyClosure> GetOrCreateAsync(DateOnly date, Guid customerId, CancellationToken token);
    Task UpdateDailyClosureValueAsync(Guid dailyClosureId, decimal transactionValue, Direction direction, CancellationToken token);
}