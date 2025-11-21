using CashFlow.Transaction.Domain.Entities;
using CashFlow.Transaction.Domain.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Transaction.Data;

public class Repository : IRepository
{
    IMongoCollection<Account> _accounts;

    public Repository(IOptions<MongoDbConfiguration>  mongoOptions)
    {
        var config = mongoOptions.Value;
        var connectionString = $"mongodb://{config.Username}:{config.Password}@{config.Host}:{config.Port}/{config.Database}?authSource={config.Username}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(config.Database);
        _accounts = database.GetCollection<Account>("accounts");
    }
    
    
    public async Task UpsertAsync(Account account, CancellationToken token)
    {
        await _accounts.ReplaceOneAsync(
            x => x.Id == account.Id,
            account,
            new ReplaceOptions { IsUpsert = true },
            token
        );
    }
    
    public async Task<Account> GetByIdAsync(Guid id)
    {
        var account = await _accounts
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();

        return account;
    }
    
    public async Task<List<Account>> SearchAsync(Guid? accountId = null)
    {
        var accounts = await _accounts
            .Find(Builders<Domain.Entities.Account>.Filter.Empty)
            .ToListAsync();

        return accounts;
    }
}