using CashFlow.Customers.Application.Requests;
using CashFlow.Customers.Application.Responses;

namespace CashFlow.Customers.Application;

public interface ICreateCustomer
{
    Task<CreateCustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct);
}