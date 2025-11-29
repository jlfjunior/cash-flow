using CashFlow.Consolidation.Application.Responses;

namespace CashFlow.Consolidation.Application;

public interface ICreateTransaction
{
    Task ExecuteAsync(TransactionCreated transactionCreated);
}