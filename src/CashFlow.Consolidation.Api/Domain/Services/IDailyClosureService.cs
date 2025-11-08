using CashFlow.Consolidation.Api.Application;

namespace CashFlow.Consolidation.Api.Domain.Services;

public interface IDailyClosureService
{
    Task<DailyClosure> GetOrCreateAsync(DateOnly date);
    Task AddTransaction(TransactionCreated dto);
}