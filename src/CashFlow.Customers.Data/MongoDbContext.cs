using MongoDB.Driver;

namespace CashFlow.Customers.Data;

public abstract class MongoDbContext
{
    protected IMongoDatabase Database { get; }

    protected MongoDbContext(IMongoClient mongoClient, string databaseName)
    {
        Database = mongoClient.GetDatabase(databaseName);
    }
}

