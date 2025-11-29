using CashFlow.Consolidation.Domain.Entities;

namespace CashFlow.Consolidation.Application;

public interface IDailyClosureService
{
    Task<DailyClosure> GetOrCreateAsync(DateOnly date);
}