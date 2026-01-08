using CashFlow.Transactions.Domain.Entities;
using MongoDB.Driver;

namespace CashFlow.Transactions.Data;

public class TransactionMongoContext : MongoDbContext
{
    public IMongoCollection<Account> Accounts { get; }

    public TransactionMongoContext(IMongoClient mongoClient, string databaseName) 
        : base(mongoClient, databaseName)
    {
        Accounts = Database.GetCollection<Account>("Accounts");
    }
}

