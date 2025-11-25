namespace CashFlow.Transactions.Application;

public interface ICreateAccount
{
    Task ExecuteAsync(Guid customerId, CancellationToken token);
}