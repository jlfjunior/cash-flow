namespace CashFlow.Transaction.Api.Domain.Services;

public interface IAccountService
{
    Task CreateAsync(Guid customerId);
}