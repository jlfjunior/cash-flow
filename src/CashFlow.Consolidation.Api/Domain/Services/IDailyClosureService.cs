using CashFlow.Consolidation.Api.Sharable;

namespace CashFlow.Consolidation.Api.Domain.Services;

public interface IDailyClosureService
{
    Task<DailyClosure> GetOrCreateAsync(DateOnly date);
    Task AddTransaction(TransactionCreated dto);
}