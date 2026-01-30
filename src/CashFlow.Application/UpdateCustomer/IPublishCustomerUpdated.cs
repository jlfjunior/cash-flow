namespace CashFlow.Features.UpdateCustomer;

public interface IPublishCustomerUpdated
{
    Task ExecuteAsync(CancellationToken token);
}
