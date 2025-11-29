using CashFlow.Consolidation.Application.Responses;
using CashFlow.Consolidation.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CashFlow.Consolidation.Application;

public class CreateTransaction(ILogger<CreateTransaction> logger) : ICreateTransaction
{
    public async Task ExecuteAsync(TransactionCreated transactionCreated)
    {
        var transaction = new Transaction
        {
            Direction = transactionCreated.Direction == "Credit" ? Direction.Credit : Direction.Debit,
            Id = transactionCreated.Id,
            DailyClosureId = Guid.CreateVersion7(),
            CustomerId = transactionCreated.CustomerId,
            ReferenceDate = DateOnly.FromDateTime(transactionCreated.ReferenceDate),
            Value = transactionCreated.Value
        };

        logger.LogInformation("Added transaction {TransactionId} to daily closure {DailyClosureId}", transaction.Id, transaction.DailyClosureId);
    }
}