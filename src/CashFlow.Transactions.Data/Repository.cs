using CashFlow.Transactions.Domain.Entities;
using CashFlow.Transactions.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Transactions.Data;

public class Repository(TransactionContext context) : IRepository
{
    public async Task UpsertAsync(Account account, CancellationToken token)
    {
        await context.SaveChangesAsync(token);
    }
    
    public async Task<Account> GetByIdAsync(Guid id)
    {
        var account = await context.Accounts
            .FirstOrDefaultAsync(x => x.Id == id);

        return account;
    }
    
    public async Task<List<Account>> SearchAsync(Guid? accountId = null)
    {
        var accounts = await context.Accounts
            .ToListAsync();

        return accounts;
    }
}