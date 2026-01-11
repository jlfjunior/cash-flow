using CashFlow.Domain.Entities;

namespace CashFlow.Features.Consolidation;

public interface IDailyClosureService
{
    Task<DailyClosure> GetOrCreateAsync(DateOnly date, Guid customerId, CancellationToken token);
    Task UpdateDailyClosureValueAsync(Guid dailyClosureId, decimal transactionValue, Direction direction, CancellationToken token);
}
