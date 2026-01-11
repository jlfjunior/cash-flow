using CashFlow.Features.Customers.Requests;
using CashFlow.Features.Customers.Responses;

namespace CashFlow.Features.Customers;

public interface ICreateCustomer
{
    Task<CreateCustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct);
}
