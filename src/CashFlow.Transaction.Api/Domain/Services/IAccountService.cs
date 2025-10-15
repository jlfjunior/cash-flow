using CashFlow.Transaction.Api.Sharable.Responses;

namespace CashFlow.Transaction.Api.Domain.Services;

public interface IAccountService
{
    Task CreateAsync(Guid customerId);
}