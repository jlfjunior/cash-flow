using CashFlow.Transactions.Application.Requests;
using CashFlow.Transactions.Application.Responses;

namespace CashFlow.Transactions.Application;

public interface IPayBill
{
    Task<AccountResponse> ExecuteAsync(PayBillRequest request, CancellationToken token);
}


