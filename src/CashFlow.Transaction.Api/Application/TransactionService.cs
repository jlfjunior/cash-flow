using CashFlow.Transaction.Api.Domain;
using CashFlow.Transaction.Api.Application.Models;

namespace CashFlow.Transaction.Api.Application;

public class TransactionService
{
    public TransactionResponse CreateCredit(Guid customerId, decimal value, DateTime? referenceDate = null)
    {
        var transaction = new Domain.Transaction(
            id: Guid.NewGuid(),
            customerId: customerId,
            type: TransactionType.Credit,
            value: value,
            referenceDate: referenceDate ?? DateTime.UtcNow
        );
        
        return MapToResponse(transaction);
    }

    public TransactionResponse CreateDebit(Guid customerId, decimal value, DateTime? referenceDate = null)
    {
        var transaction = new Domain.Transaction(
            id: Guid.NewGuid(),
            customerId: customerId,
            type: TransactionType.Debit,
            value: value,
            referenceDate: referenceDate ?? DateTime.UtcNow
        );
        
        return MapToResponse(transaction);
    }

    public List<TransactionResponse> Search(Guid customerId)
    {
        // Mock data for development and testing
        var transactions = new List<Domain.Transaction>
        {
            new Domain.Transaction(
                id: Guid.NewGuid(),
                customerId: customerId,
                type: TransactionType.Credit,
                value: 1500.00m,
                referenceDate: DateTime.UtcNow.AddDays(-5)
            ),
            new Domain.Transaction(
                id: Guid.NewGuid(),
                customerId: customerId,
                type: TransactionType.Debit,
                value: 250.75m,
                referenceDate: DateTime.UtcNow.AddDays(-3)
            ),
            new Domain.Transaction(
                id: Guid.NewGuid(),
                customerId: customerId,
                type: TransactionType.Credit,
                value: 800.00m,
                referenceDate: DateTime.UtcNow.AddDays(-1)
            ),
            new Domain.Transaction(
                id: Guid.NewGuid(),
                customerId: customerId,
                type: TransactionType.Debit,
                value: 120.50m,
                referenceDate: DateTime.UtcNow.AddHours(-6)
            ),
            new Domain.Transaction(
                id: Guid.NewGuid(),
                customerId: customerId,
                type: TransactionType.Credit,
                value: 2000.00m,
                referenceDate: DateTime.UtcNow.AddHours(-2)
            )
        };
        
        // In a real application, this would query a database or repository:
        // var query = _transactionRepository.GetAll();
        // query = query.Where(t => t.CustomerId == customerId);
        // return query.ToList();
        
        return transactions.Select(MapToResponse).ToList();
    }

    private static TransactionResponse MapToResponse(Domain.Transaction transaction)
    {
        return new TransactionResponse
        {
            Id = transaction.Id,
            CustomerId = transaction.CustomerId,
            Type = transaction.Type.ToString(),
            ReferenceDate = transaction.ReferenceDate,
            Value = transaction.Value
        };
    }
}
