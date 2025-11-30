namespace CashFlow.Customers.Application;

public interface IPublishCustomerUpdated
{
    Task ExecuteAsync(CancellationToken token);
}