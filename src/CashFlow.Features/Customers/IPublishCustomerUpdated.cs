namespace CashFlow.Features.Customers;

public interface IPublishCustomerUpdated
{
    Task ExecuteAsync(CancellationToken token);
}
