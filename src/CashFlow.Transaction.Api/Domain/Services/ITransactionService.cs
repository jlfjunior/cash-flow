using CashFlow.Transaction.Api.Sharable.Responses;

namespace CashFlow.Transaction.Api.Domain.Services;

public interface ITransactionService
{
    Task<TransactionResponse> CreateCreditAsync(Guid customerId, decimal value);
    Task<TransactionResponse> CreateDebitAsync(Guid customerId, decimal value);
    Task<List<TransactionResponse>> SearchAsync(Guid? customerId = null);
}