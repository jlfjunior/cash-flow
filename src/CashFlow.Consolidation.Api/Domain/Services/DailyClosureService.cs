using CashFlow.Consolidation.Api.Sharable;

namespace CashFlow.Consolidation.Api.Domain.Services;

public class DailyClosureService : IDailyClosureService
{
    private readonly ILogger<DailyClosureService> _logger;

    public DailyClosureService(ILogger<DailyClosureService> logger)
    {
        _logger = logger;
    }

    public Task<DailyClosure> GetOrCreateAsync(DateOnly date)
    {
        throw new NotImplementedException();
    }

    public async Task AddTransaction(TransactionDto dto)
    {
        var dailyClosure = await GetOrCreateAsync(dto.ReferenceDate);
        
        var transaction = new Transaction
        {
            Direction = dto.Direction == "Credit" ? Direction.Credit : Direction.Debit,
            Id = dto.Id,
            DailyClosureId = dailyClosure.Id,
            CustomerId = dto.CustomerId,
            ReferenceDate = dto.ReferenceDate,
            Value = dto.Value
        };
        
        _logger.LogInformation("Adding new transaction {@transaction}", transaction);
    }
}