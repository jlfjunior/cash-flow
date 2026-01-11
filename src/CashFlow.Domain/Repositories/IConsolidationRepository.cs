using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories;

public interface IConsolidationRepository
{
    Task UpsertAsync(ConsolidationTransaction transaction, CancellationToken token);
    Task UpsertAsync(DailyClosure dailyClosure, CancellationToken token);
    Task<DailyClosure?> GetDailyClosureByDateAsync(DateOnly date, Guid customerId, CancellationToken token);
    Task<List<ConsolidationTransaction>> GetTransactionsByDailyClosureIdAsync(Guid dailyClosureId, CancellationToken token);
}
