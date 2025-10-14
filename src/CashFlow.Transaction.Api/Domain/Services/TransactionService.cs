using CashFlow.Lib.EventBus;
using CashFlow.Transaction.Api.Domain.Entities;
using CashFlow.Transaction.Api.Domain.Events;
using CashFlow.Transaction.Api.Infrastructure;
using CashFlow.Transaction.Api.Sharable.Responses;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Transaction.Api.Domain.Services;

public class TransactionService : ITransactionService
{
    private readonly ILogger<TransactionService> _logger;
    private readonly IMongoCollection<Entities.Transaction> _transactions;
    private readonly IEventBus _eventBus;

    public TransactionService(ILogger<TransactionService> logger, IEventBus eventBus, IOptions<MongoDbConfiguration> mongoOptions)
    {
        _logger = logger;
        _eventBus = eventBus;
        
        var config = mongoOptions.Value;
        var connectionString = $"mongodb://{config.Username}:{config.Password}@{config.Host}:{config.Port}/{config.Database}?authSource={config.Username}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(config.Database);
        _transactions = database.GetCollection<Entities.Transaction>("transactions");
    }
    
    public async Task<TransactionResponse> CreateCreditAsync(Guid customerId, decimal value)
    {
        var transaction = new Entities.Transaction(
            id: Guid.CreateVersion7(),
            customerId: customerId,
            direction: Direction.Credit,
            value: value
        );

        _transactions.InsertOne(transaction);
        
        var response = new TransactionResponse
        {
            Id = transaction.Id,
            CustomerId = transaction.CustomerId,
            Direction = transaction.Direction.ToString(),
            ReferenceDate = transaction.ReferenceDate,
            Value = transaction.Value
        };

        var @event = new TransactionCreated(
            transaction.Id,
            transaction.CustomerId,
            transaction.Direction.ToString(),
            transaction.ReferenceDate,
            transaction.Value);
        
        transaction.AddEvent(@event);
        await _eventBus.PublishAsync(@event, "transaction.created");
        
        _logger.LogInformation($"Transaction Created. Id: {response.Id}");
        
        return response;
    }

    public async Task<TransactionResponse> CreateDebitAsync(Guid customerId, decimal value)
    {
        var transaction = new Entities.Transaction(
            id: Guid.CreateVersion7(),
            customerId: customerId,
            direction: Direction.Debit,
            value: value
        );
        
        _transactions.InsertOne(transaction);
        
        var response = new TransactionResponse
        {
            Id = transaction.Id,
            CustomerId = transaction.CustomerId,
            Direction = transaction.Direction.ToString(),
            ReferenceDate = transaction.ReferenceDate,
            Value = transaction.Value
        };
        
        var @event = new TransactionCreated(
            transaction.Id,
            transaction.CustomerId,
            transaction.Direction.ToString(),
            transaction.ReferenceDate,
            transaction.Value);
        
        transaction.AddEvent(@event);
        
        await _eventBus.PublishAsync(@event, "transaction.created");

        _logger.LogInformation($"Transaction Created. Id: {response.Id}");

        return response;
    }

    public async Task<List<TransactionResponse>> SearchAsync(Guid? customerId = null)
    {
        var transactions = await _transactions
            .Find(Builders<Domain.Entities.Transaction>.Filter.Empty)
            .Project(t => new TransactionResponse
            {
                Id = t.Id,
                CustomerId = t.CustomerId,
                Direction = t.Direction.ToString(),
                ReferenceDate = t.ReferenceDate,
                Value = t.Value
            })
            .ToListAsync();

        return transactions;
    }
}
