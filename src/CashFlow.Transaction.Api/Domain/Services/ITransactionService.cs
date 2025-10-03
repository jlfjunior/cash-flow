using CashFlow.Transaction.Api.Sharable;
using CashFlow.Transaction.Api.Sharable.Responses;

namespace CashFlow.Transaction.Api.Domain.Services;

public interface ITransactionService
{
    TransactionResponse CreateCredit(Guid customerId, decimal value, DateTime? referenceDate = null);
    TransactionResponse CreateDebit(Guid customerId, decimal value, DateTime? referenceDate = null);
    List<TransactionResponse> Search(Guid? customerId = null);
}