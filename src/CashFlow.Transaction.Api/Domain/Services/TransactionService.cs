using CashFlow.Transaction.Api.Domain.Entities;
using CashFlow.Transaction.Api.Domain.Events;
using CashFlow.Transaction.Api.Infrastructure.EventBus;
using CashFlow.Transaction.Api.Sharable.Responses;
using MongoDB.Driver;

namespace CashFlow.Transaction.Api.Domain.Services;

public class TransactionService : ITransactionService
{
    private readonly ILogger<TransactionService> _logger;
    private readonly IMongoCollection<Entities.Transaction> _transactions;
    private readonly IEventBus _eventBus;

    public TransactionService(ILogger<TransactionService> logger, IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
        
        var mongoClient = new MongoClient("mongodb://admin:password123@localhost:27017/cashflow?authSource=admin");
        var database = mongoClient.GetDatabase("cashflow");
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

        await _eventBus.PublishAsync(transaction.DomainEvents);
        
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
        
        await _eventBus.PublishAsync(transaction.DomainEvents);

        _logger.LogInformation($"Transaction Created. Id: {response.Id}");

        return response;
    }

    public async Task<List<TransactionResponse>> SearchAsync(Guid? customerId = null)
    {
        // Mock data for development and testing
        var transactions = new List<Entities.Transaction>
        {
            new (id: Guid.CreateVersion7(), customerId: customerId ?? Guid.NewGuid(), direction: Direction.Credit, value: 1500.00m),
            new (id: Guid.CreateVersion7(), customerId: customerId ?? Guid.NewGuid(), direction: Direction.Debit, value: 250.75m),
            new (id: Guid.CreateVersion7(), customerId: customerId ?? Guid.NewGuid(), direction: Direction.Credit, value: 800.00m),
            new (id: Guid.CreateVersion7(), customerId: customerId ?? Guid.NewGuid(), direction: Direction.Debit, value: 120.50m),
            new (id: Guid.CreateVersion7(), customerId: customerId ?? Guid.NewGuid(), direction: Direction.Credit, value: 2000.00m)
        };
        
        return transactions.Select(t => new TransactionResponse
        {
            Id = t.Id,
            CustomerId = t.CustomerId,
            Direction = t.Direction.ToString(),
            ReferenceDate = t.ReferenceDate,
            Value = t.Value
        }).ToList();
    }
}
