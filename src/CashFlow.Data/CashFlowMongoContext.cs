using CashFlow.Domain.Entities;
using MongoDB.Driver;

namespace CashFlow.Data;

public class CashFlowMongoContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<Customer> Customers { get; }
    public IMongoCollection<OutboxMessage> OutboxMessages { get; }
    public IMongoCollection<Account> Accounts { get; }
    public IMongoCollection<DailyClosure> DailyClosures { get; }
    public IMongoCollection<ConsolidationTransaction> ConsolidationTransactions { get; }

    public CashFlowMongoContext(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
        Customers = _database.GetCollection<Customer>("Customers");
        OutboxMessages = _database.GetCollection<OutboxMessage>("OutboxMessages");
        Accounts = _database.GetCollection<Account>("Accounts");
        DailyClosures = _database.GetCollection<DailyClosure>("daily_closures");
        ConsolidationTransactions = _database.GetCollection<ConsolidationTransaction>("transactions");
    }
}
