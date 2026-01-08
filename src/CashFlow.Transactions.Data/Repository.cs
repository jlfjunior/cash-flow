using CashFlow.Transactions.Domain.Entities;
using CashFlow.Transactions.Domain.Repositories;
using MongoDB.Driver;

namespace CashFlow.Transactions.Data;

public class Repository : IRepository
{
    private readonly TransactionMongoContext _context;

    public Repository(TransactionMongoContext context)
    {
        _context = context;
    }

    public async Task UpsertAsync(Account account, CancellationToken token)
    {
        var filter = Builders<Account>.Filter.Eq(a => a.Id, account.Id);
        await _context.Accounts.ReplaceOneAsync(
            filter, 
            account, 
            new ReplaceOptions { IsUpsert = true }, 
            token);
    }
    
    public async Task<Account> GetByIdAsync(Guid id)
    {
        var filter = Builders<Account>.Filter.Eq(a => a.Id, id);
        var account = await _context.Accounts
            .Find(filter)
            .FirstOrDefaultAsync();

        return account;
    }
    
    public async Task<List<Account>> SearchAsync(Guid? accountId = null)
    {
        var filterBuilder = Builders<Account>.Filter;
        var filter = accountId.HasValue 
            ? filterBuilder.Eq(a => a.Id, accountId.Value)
            : FilterDefinition<Account>.Empty;

        var accounts = await _context.Accounts
            .Find(filter)
            .ToListAsync();

        return accounts;
    }
}

