using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories;

public interface IAccountRepository
{
    Task<Account> GetByIdAsync(Guid id);
    Task UpsertAsync(Account account, CancellationToken token);
    Task<List<Account>> SearchAsync(Guid? accountId = null);
}
