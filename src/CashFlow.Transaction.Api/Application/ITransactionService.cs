using CashFlow.Transaction.Api.Application.Models;

namespace CashFlow.Transaction.Api.Application;

public interface ITransactionService
{
    TransactionResponse CreateCredit(Guid customerId, decimal value, DateTime? referenceDate = null);
    TransactionResponse CreateDebit(Guid customerId, decimal value, DateTime? referenceDate = null);
    List<TransactionResponse> Search(Guid? customerId = null);
}