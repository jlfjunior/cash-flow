using CashFlow.Consolidation.Domain.Entities;

namespace CashFlow.Consolidation.Domain.Repositories;

public interface IRepository
{
    Task UpsertAsync(Transaction transaction, CancellationToken token);
    Task UpsertAsync(DailyClosure dailyClosure, CancellationToken token);
    Task<DailyClosure?> GetDailyClosureByDateAsync(DateOnly date, Guid customerId, CancellationToken token);
    Task<List<Transaction>> GetTransactionsByDailyClosureIdAsync(Guid dailyClosureId, CancellationToken token);
}

