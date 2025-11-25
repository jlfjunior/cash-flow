using CashFlow.Customers.Application.Requests;
using CashFlow.Customers.Application.Responses;

namespace CashFlow.Customers.Application;

public interface IUpdateCustomer
{
    Task<UpdateCustomerResponse> ExecuteAsync(UpdateCustomerRequest request,  CancellationToken token);
}