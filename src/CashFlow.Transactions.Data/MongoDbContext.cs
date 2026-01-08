using MongoDB.Driver;

namespace CashFlow.Transactions.Data;

public abstract class MongoDbContext
{
    protected IMongoDatabase Database { get; }

    protected MongoDbContext(IMongoClient mongoClient, string databaseName)
    {
        Database = mongoClient.GetDatabase(databaseName);
    }
}

