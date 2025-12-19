using CashFlow.Transactions.Application.Requests;
using CashFlow.Transactions.Application.Responses;

namespace CashFlow.Transactions.Application;

public interface ITransfer
{
    Task<AccountResponse> ExecuteAsync(TransferRequest request, CancellationToken token);
}

