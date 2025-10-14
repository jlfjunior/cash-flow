using CashFlow.Lib.EventBus;
using CashFlow.Transaction.Api.Domain.Entities;
using CashFlow.Transaction.Api.Domain.Events;
using CashFlow.Transaction.Api.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Transaction.Api.Domain.Services;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IMongoCollection<Account> _accounts;
    private readonly IEventBus _eventBus;

    public AccountService(ILogger<AccountService> logger, IEventBus eventBus, IOptions<MongoDbConfiguration> mongoOptions)
    {
        _logger = logger;
        _eventBus = eventBus;
        
        var config = mongoOptions.Value;
        var connectionString = $"mongodb://{config.Username}:{config.Password}@{config.Host}:{config.Port}/{config.Database}?authSource={config.Username}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(config.Database);
        _accounts = database.GetCollection<Account>("accounts");
    }
    
    public async Task CreateAsync(Guid customerId)
    {
        var account = new Account(customerId);
        
        _accounts.InsertOne(account);
        
        var @event = new AccountCreated(account.Id, account.CustomerId);
        
        account.AddEvent(@event);
        
        await _eventBus.PublishAsync(@event, "account.created");
        
        _logger.LogInformation($"Account created for customer: {customerId}");

        await Task.CompletedTask;
    }
}