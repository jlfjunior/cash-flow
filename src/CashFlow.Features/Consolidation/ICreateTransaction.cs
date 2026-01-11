using CashFlow.Features.Consolidation.Responses;

namespace CashFlow.Features.Consolidation;

public interface ICreateTransaction
{
    Task ExecuteAsync(TransactionCreated transactionCreated);
}
