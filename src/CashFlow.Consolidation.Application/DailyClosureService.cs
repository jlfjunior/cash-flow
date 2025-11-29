using CashFlow.Consolidation.Data;
using CashFlow.Consolidation.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Transaction = System.Transactions.Transaction;

namespace CashFlow.Consolidation.Application;

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
}