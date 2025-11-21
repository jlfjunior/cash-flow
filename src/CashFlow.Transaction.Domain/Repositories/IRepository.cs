using CashFlow.Transaction.Domain.Entities;

namespace CashFlow.Transaction.Domain.Repositories
{
    public interface IRepository
    {
        Task<Account> GetByIdAsync(Guid id);
        Task UpsertAsync(Account account, CancellationToken token);
        Task<List<Account>> SearchAsync(Guid? accountId = null);
    }
}