using CashFlow.Consolidation.Domain.Entities;
using MongoDB.Driver;

namespace CashFlow.Consolidation.Data;

public class ConsolidationMongoContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<DailyClosure> DailyClosures { get; }
    public IMongoCollection<Transaction> Transactions { get; }

    public ConsolidationMongoContext(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
        DailyClosures = _database.GetCollection<DailyClosure>("daily_closures");
        Transactions = _database.GetCollection<Transaction>("transactions");
    }
}

