using CashFlow.Transaction.Api.Domain.Entities;
using CashFlow.Transaction.Api.Domain.Events;
using CashFlow.Transaction.Api.Sharable;
using CashFlow.Transaction.Api.Sharable.Responses;
using MongoDB.Driver;

namespace CashFlow.Transaction.Api.Domain.Services;

public class TransactionService : ITransactionService
{
    private readonly ILogger<TransactionService> _logger;
    private readonly IMongoCollection<Entities.Transaction> _transactions;
    private readonly IEventPublisher _eventPublisher;

    public TransactionService(ILogger<TransactionService> logger, IEventPublisher eventPublisher)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
        
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var database = mongoClient.GetDatabase("cashflow");
        _transactions = database.GetCollection<Entities.Transaction>("transactions");
    }
    
    public TransactionResponse CreateCredit(Guid customerId, decimal value, DateTime? referenceDate = null)
    {
        var transaction = new Entities.Transaction(
            id: Guid.NewGuid(),
            customerId: customerId,
            type: TransactionType.Credit,
            value: value,
            referenceDate: referenceDate ?? DateTime.UtcNow
        );
        
        _transactions.InsertOne(transaction);
        
        var response = new TransactionResponse
        {
            Id = transaction.Id,
            CustomerId = transaction.CustomerId,
            Type = transaction.Type.ToString(),
            ReferenceDate = transaction.ReferenceDate,
            Value = transaction.Value
        };
        
        var domainEvent = new TransactionCreatedEvent(
            transaction.Id,
            transaction.CustomerId,
            transaction.Type.ToString(),
            transaction.ReferenceDate,
            transaction.Value);
        
        // Publish event to RabbitMQ
        _ = Task.Run(async () =>
        {
            try
            {
                await _eventPublisher.PublishAsync(domainEvent, "transaction.created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish TransactionCreatedEvent for transaction {TransactionId}", transaction.Id);
            }
        });
        
        _logger.LogInformation($"Transaction Created. Id: {response.Id}");
        
        return response;
    }

    public TransactionResponse CreateDebit(Guid customerId, decimal value, DateTime? referenceDate = null)
    {
        var transaction = new Entities.Transaction(
            id: Guid.NewGuid(),
            customerId: customerId,
            type: TransactionType.Debit,
            value: value,
            referenceDate: referenceDate ?? DateTime.UtcNow
        );
        
        _transactions.InsertOne(transaction);
        
        var response = new TransactionResponse
        {
            Id = transaction.Id,
            CustomerId = transaction.CustomerId,
            Type = transaction.Type.ToString(),
            ReferenceDate = transaction.ReferenceDate,
            Value = transaction.Value
        };
        
        var domainEvent = new TransactionCreatedEvent(
            transaction.Id,
            transaction.CustomerId,
            transaction.Type.ToString(),
            transaction.ReferenceDate,
            transaction.Value);
        
        // Publish event to RabbitMQ
        _ = Task.Run(async () =>
        {
            try
            {
                await _eventPublisher.PublishAsync(domainEvent, "transaction.created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish TransactionCreatedEvent for transaction {TransactionId}", transaction.Id);
            }
        });
        
        _logger.LogInformation($"Transaction Created. Id: {response.Id}");

        return response;
    }

    public List<TransactionResponse> Search(Guid? customerId = null)
    {
        // Mock data for development and testing
        var transactions = new List<Entities.Transaction>
        {
            new Entities.Transaction(
                id: Guid.NewGuid(),
                customerId: customerId ?? Guid.NewGuid(),
                type: TransactionType.Credit,
                value: 1500.00m,
                referenceDate: DateTime.UtcNow.AddDays(-5)
            ),
            new Entities.Transaction(
                id: Guid.NewGuid(),
                customerId: customerId ?? Guid.NewGuid(),
                type: TransactionType.Debit,
                value: 250.75m,
                referenceDate: DateTime.UtcNow.AddDays(-3)
            ),
            new Entities.Transaction(
                id: Guid.NewGuid(),
                customerId: customerId ?? Guid.NewGuid(),
                type: TransactionType.Credit,
                value: 800.00m,
                referenceDate: DateTime.UtcNow.AddDays(-1)
            ),
            new Entities.Transaction(
                id: Guid.NewGuid(),
                customerId: customerId ?? Guid.NewGuid(),
                type: TransactionType.Debit,
                value: 120.50m,
                referenceDate: DateTime.UtcNow.AddHours(-6)
            ),
            new Entities.Transaction(
                id: Guid.NewGuid(),
                customerId: customerId ?? Guid.NewGuid(),
                type: TransactionType.Credit,
                value: 2000.00m,
                referenceDate: DateTime.UtcNow.AddHours(-2)
            )
        };
        
        // In a real application, this would query a database or repository:
        // var query = _transactionRepository.GetAll();
        // query = query.Where(t => t.CustomerId == customerId);
        // return query.ToList();
        
        return transactions.Select(t => new TransactionResponse
        {
            Id = t.Id,
            CustomerId = t.CustomerId,
            Type = t.Type.ToString(),
            ReferenceDate = t.ReferenceDate,
            Value = t.Value
        }).ToList();
    }
}
