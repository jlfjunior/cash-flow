namespace CashFlow.Features.CreateCustomer;

public interface ICreateCustomer
{
    Task<CreateCustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct);
}
