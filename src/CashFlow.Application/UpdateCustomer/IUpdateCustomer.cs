namespace CashFlow.Features.UpdateCustomer;

public interface IUpdateCustomer
{
    Task<UpdateCustomerResponse> ExecuteAsync(UpdateCustomerRequest request,  CancellationToken token);
}
