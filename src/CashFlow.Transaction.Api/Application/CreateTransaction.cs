using CashFlow.Lib.EventBus;
using CashFlow.Transaction.Api.Application.Requests;
using CashFlow.Transaction.Api.Application.Responses;
using CashFlow.Transaction.Api.Domain.Entities;
using CashFlow.Transaction.Api.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Transaction.Api.Application;

public interface ICreateTransaction
{
    Task<AccountResponse> ExecuteAsync(CreateTransactionRequest request);
}

public class CreateTransaction : ICreateTransaction
{
    private readonly ILogger<CreateTransaction> _logger;
    private readonly IMongoCollection<Account> _accounts;
    private readonly IEventBus _eventBus;
    
    public CreateTransaction(ILogger<CreateTransaction> logger, IOptions<MongoDbConfiguration> mongoOptions,  IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
        
        var config = mongoOptions.Value;
        var connectionString = $"mongodb://{config.Username}:{config.Password}@{config.Host}:{config.Port}/{config.Database}?authSource={config.Username}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(config.Database);
        _accounts = database.GetCollection<Account>("accounts");
    }
    
    public async Task<AccountResponse> ExecuteAsync(CreateTransactionRequest request)
    {
        var account = await _accounts
            .Find(x => x.Id == request.AccountId)
            .SingleAsync();

        account.AddTransaction(request.Direction, request.Value);
        
        _accounts.InsertOne(account);
        
        await _eventBus.PublishAsync(account.Transactions.First(), "accounts.update");
        
        _logger.LogInformation("Transaction created.  Id {AccountId}.", account.Id);

        return new AccountResponse(account.Id);
    }
}