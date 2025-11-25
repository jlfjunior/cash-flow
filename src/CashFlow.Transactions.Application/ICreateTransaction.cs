using CashFlow.Transactions.Application.Requests;
using CashFlow.Transactions.Application.Responses;

namespace CashFlow.Transactions.Application;

public interface ICreateTransaction
{
    Task<AccountResponse> ExecuteAsync(CreateTransactionRequest request, CancellationToken token);
}