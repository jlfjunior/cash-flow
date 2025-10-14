using CashFlow.Consolidation.Api.Infrastructure;
using CashFlow.Consolidation.Api.Sharable;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Consolidation.Api.Domain.Services;

public class DailyClosureService : IDailyClosureService
{
    private readonly ILogger<DailyClosureService> _logger;
    private readonly IMongoCollection<DailyClosure> _dailyClosures;
    private readonly IMongoCollection<Transaction> _transactions;

    public DailyClosureService(ILogger<DailyClosureService> logger, IOptions<MongoDbConfiguration> mongoOptions)
    {
        _logger = logger;
        
        var config = mongoOptions.Value;
        var connectionString = $"mongodb://{config.Username}:{config.Password}@{config.Host}:{config.Port}/{config.Database}?authSource={config.Username}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(config.Database);
        _dailyClosures = database.GetCollection<DailyClosure>("daily_closures");
        _transactions = database.GetCollection<Transaction>("transactions");
    }

    public async Task<DailyClosure> GetOrCreateAsync(DateOnly date)
    {
        var filter = Builders<DailyClosure>.Filter.Eq(dc => dc.ReferenceDate, date);
        var existingClosure = await _dailyClosures.Find(filter).FirstOrDefaultAsync();

        if (existingClosure != null)
        {
            return existingClosure;
        }

        var newClosure = new DailyClosure
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.Empty, // Will be set when adding transactions
            ReferenceDate = date,
            Value = 0
        };

        await _dailyClosures.InsertOneAsync(newClosure);
        _logger.LogInformation("Created new daily closure for date {Date}", date);

        return newClosure;
    }

    public async Task AddTransaction(TransactionCreated dto)
    {
        var transaction = new Transaction
        {
            Direction = dto.Direction == "Credit" ? Direction.Credit : Direction.Debit,
            Id = dto.Id,
            DailyClosureId = Guid.CreateVersion7(),
            CustomerId = dto.CustomerId,
            ReferenceDate = DateOnly.FromDateTime(dto.ReferenceDate),
            Value = dto.Value
        };
        
        _logger.LogInformation("Added transaction {TransactionId} to daily closure {DailyClosureId}", 
            transaction.Id, transaction.DailyClosureId);
    }
}