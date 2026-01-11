using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CashFlow.Features.Consolidation;

public class DailyClosureService : IDailyClosureService
{
    private readonly ILogger<DailyClosureService> _logger;
    private readonly IConsolidationRepository _repository;

    public DailyClosureService(ILogger<DailyClosureService> logger, IConsolidationRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<DailyClosure> GetOrCreateAsync(DateOnly date, Guid customerId, CancellationToken token)
    {
        var existingClosure = await _repository.GetDailyClosureByDateAsync(date, customerId, token);

        if (existingClosure != null)
        {
            return existingClosure;
        }

        var newClosure = new DailyClosure
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ReferenceDate = date,
            Value = 0
        };

        await _repository.UpsertAsync(newClosure, token);
        _logger.LogInformation("Created new daily closure for date {Date} and customer {CustomerId}", date, customerId);

        return newClosure;
    }

    public async Task UpdateDailyClosureValueAsync(Guid dailyClosureId, decimal transactionValue, Direction direction, CancellationToken token)
    {
        // Get all transactions for this daily closure
        var transactions = await _repository.GetTransactionsByDailyClosureIdAsync(dailyClosureId, token);
        
        if (!transactions.Any())
        {
            _logger.LogWarning("No transactions found for daily closure {DailyClosureId}", dailyClosureId);
            return;
        }

        // Recalculate total value from all transactions
        // Credits increase the value, debits decrease it
        var totalValue = transactions
            .Where(t => t.Direction == Direction.Credit)
            .Sum(t => t.Value) - transactions
            .Where(t => t.Direction == Direction.Debit)
            .Sum(t => t.Value);

        // Get the daily closure to update
        var firstTransaction = transactions.First();
        var dailyClosure = await _repository.GetDailyClosureByDateAsync(
            firstTransaction.ReferenceDate, 
            firstTransaction.CustomerId, 
            token);

        if (dailyClosure == null)
        {
            _logger.LogError("Daily closure {DailyClosureId} not found", dailyClosureId);
            return;
        }

        dailyClosure.Value = totalValue;
        await _repository.UpsertAsync(dailyClosure, token);
        
        _logger.LogInformation(
            "Updated daily closure {DailyClosureId} value to {Value} (recalculated from {TransactionCount} transactions)",
            dailyClosureId, 
            totalValue,
            transactions.Count);
    }
}
