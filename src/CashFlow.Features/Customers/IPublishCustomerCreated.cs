namespace CashFlow.Features.Customers;

public interface IPublishCustomerCreated
{
    Task ExecuteAsync(CancellationToken token);
}
