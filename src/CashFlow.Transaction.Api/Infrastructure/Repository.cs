using CashFlow.Transaction.Api.Application.Responses;
using CashFlow.Transaction.Api.Domain.Entities;
using CashFlow.Transaction.Api.Domain.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Transaction.Api.Infrastructure;

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
    
    public async Task<List<AccountResponse>> SearchAsync(Guid? accountId = null)
    {
        var accounts = await _accounts
            .Find(Builders<Domain.Entities.Account>.Filter.Empty)
            .Project(t => new AccountResponse(t.Id))
            .ToListAsync();

        return accounts;
    }
}