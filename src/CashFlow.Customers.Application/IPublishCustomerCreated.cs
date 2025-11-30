namespace CashFlow.Customers.Application;

public interface IPublishCustomerCreated
{
    Task ExecuteAsync(CancellationToken token);
}