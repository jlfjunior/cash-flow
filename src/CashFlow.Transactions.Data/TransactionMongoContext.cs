using CashFlow.Transactions.Domain.Entities;
using MongoDB.Driver;

namespace CashFlow.Transactions.Data;

public class TransactionMongoContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<Account> Accounts { get; }

    public TransactionMongoContext(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
        Accounts = _database.GetCollection<Account>("Accounts");
    }
}

