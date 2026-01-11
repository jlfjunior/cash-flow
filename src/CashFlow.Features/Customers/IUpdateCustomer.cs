using CashFlow.Features.Customers.Requests;
using CashFlow.Features.Customers.Responses;

namespace CashFlow.Features.Customers;

public interface IUpdateCustomer
{
    Task<UpdateCustomerResponse> ExecuteAsync(UpdateCustomerRequest request,  CancellationToken token);
}
