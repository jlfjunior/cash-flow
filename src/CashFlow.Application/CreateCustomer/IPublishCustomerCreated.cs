namespace CashFlow.Features.CreateCustomer;

public interface IPublishCustomerCreated
{
    Task ExecuteAsync(CancellationToken token);
}
