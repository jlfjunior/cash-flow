using CashFlow.Transaction.Api.Sharable;

namespace CashFlow.Transaction.Api.Domain;

public interface ITransactionService
{
    TransactionResponse CreateCredit(Guid customerId, decimal value, DateTime? referenceDate = null);
    TransactionResponse CreateDebit(Guid customerId, decimal value, DateTime? referenceDate = null);
    List<TransactionResponse> Search(Guid? customerId = null);
}