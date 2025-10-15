using CashFlow.Transaction.Api.Sharable.Responses;

namespace CashFlow.Transaction.Api.Domain.Services;

public interface ITransactionService
{
    Task<List<TransactionResponse>> SearchAsync(Guid? accountId = null);
    Task<TransactionResponse> CreateCreditAsync(Guid accountId, decimal value);
    Task<TransactionResponse> CreateDebitAsync(Guid accountId, decimal value);
}