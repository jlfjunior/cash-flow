using CashFlow.Customers.Domain.Entities;
using MongoDB.Driver;

namespace CashFlow.Customers.Data;

public class CustomerMongoContext : MongoDbContext
{
    public IMongoCollection<Customer> Customers { get; }
    public IMongoCollection<OutboxMessage> OutboxMessages { get; }

    public CustomerMongoContext(IMongoClient mongoClient, string databaseName) 
        : base(mongoClient, databaseName)
    {
        Customers = Database.GetCollection<Customer>("Customers");
        OutboxMessages = Database.GetCollection<OutboxMessage>("OutboxMessages");
    }
}

