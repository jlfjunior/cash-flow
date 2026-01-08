using CashFlow.Consolidation.Application.Responses;
using CashFlow.Consolidation.Domain.Entities;
using CashFlow.Consolidation.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CashFlow.Consolidation.Application;

public class CreateTransaction : ICreateTransaction
{
    private readonly ILogger<CreateTransaction> _logger;
    private readonly IDailyClosureService _dailyClosureService;
    private readonly IRepository _repository;

    public CreateTransaction(
        ILogger<CreateTransaction> logger,
        IDailyClosureService dailyClosureService,
        IRepository repository)
    {
        _logger = logger;
        _dailyClosureService = dailyClosureService;
        _repository = repository;
    }

    public async Task ExecuteAsync(TransactionCreated transactionCreated)
    {
        var referenceDate = DateOnly.FromDateTime(transactionCreated.ReferenceDate);
        var direction = transactionCreated.Direction == "Credit" ? Direction.Credit : Direction.Debit;

        // Get or create daily closure for this customer and date
        var dailyClosure = await _dailyClosureService.GetOrCreateAsync(
            referenceDate, 
            transactionCreated.CustomerId, 
            CancellationToken.None);

        // Create transaction entity
        var transaction = new Transaction
        {
            Direction = direction,
            Id = transactionCreated.Id,
            DailyClosureId = dailyClosure.Id,
            CustomerId = transactionCreated.CustomerId,
            ReferenceDate = referenceDate,
            Value = transactionCreated.Value
        };

        // Save transaction
        await _repository.UpsertAsync(transaction, CancellationToken.None);

        // Update daily closure value
        await _dailyClosureService.UpdateDailyClosureValueAsync(
            dailyClosure.Id, 
            transactionCreated.Value, 
            direction, 
            CancellationToken.None);

        _logger.LogInformation(
            "Added transaction {TransactionId} to daily closure {DailyClosureId} with value {Value} ({Direction})",
            transaction.Id, 
            dailyClosure.Id, 
            transactionCreated.Value, 
            direction);
    }
}