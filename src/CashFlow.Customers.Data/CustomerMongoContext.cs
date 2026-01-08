using CashFlow.Customers.Domain.Entities;
using MongoDB.Driver;

namespace CashFlow.Customers.Data;

public class CustomerMongoContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<Customer> Customers { get; }
    public IMongoCollection<OutboxMessage> OutboxMessages { get; }

    public CustomerMongoContext(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
        Customers = _database.GetCollection<Customer>("Customers");
        OutboxMessages = _database.GetCollection<OutboxMessage>("OutboxMessages");
    }
}

