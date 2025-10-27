using CashFlow.Customer.Api.Domain.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Customer.Api.Domain.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IMongoCollection<Customer> _customers;

    public CustomerRepository(IOptions<MongoDbConfiguration> mongoOptions)
    {
        var config = mongoOptions.Value;
        var connectionString =
            $"mongodb://{config.Username}:{config.Password}@{config.Host}:{config.Port}/{config.Database}?authSource={config.Username}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(config.Database);
        _customers = database.GetCollection<Customer>("customers");
    }

    public async Task<IEnumerable<CreateCustomerResponse>> SearchAsync()
    {
        var customers = await _customers
            .Find(Builders<Customer>.Filter.Empty)
            .Project(t => new CreateCustomerResponse(t.Id, t.FullName))
            .ToListAsync();

        return customers;
    }
}