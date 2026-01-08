using CashFlow.Transactions.Domain.Entities;

namespace CashFlow.Transactions.Domain.Repositories
{
    public interface IRepository
    {
        Task<Account> GetByIdAsync(Guid id);
        Task UpsertAsync(Account account, CancellationToken token);
        Task<List<Account>> SearchAsync(Guid? accountId = null);
    }
}

