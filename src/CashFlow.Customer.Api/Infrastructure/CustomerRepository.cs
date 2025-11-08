using CashFlow.Customer.Api.Application.Responses;
using CashFlow.Customer.Api.Domain.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Customer.Api.Infrastructure;

public class CustomerRepository : ICustomerRepository
{
    private readonly IMongoCollection<Domain.Entities.Customer> _customers;

    public CustomerRepository(IOptions<MongoDbConfiguration> mongoOptions)
    {
        var config = mongoOptions.Value;
        var connectionString =
            $"mongodb://{config.Username}:{config.Password}@{config.Host}:{config.Port}/{config.Database}?authSource={config.Username}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(config.Database);
        _customers = database.GetCollection<Domain.Entities.Customer>("customers");
    }

    public async Task UpsertAsync(Domain.Entities.Customer customer, CancellationToken token)
    {
        await _customers.ReplaceOneAsync(
            x => x.Id == customer.Id,
            customer,
            new ReplaceOptions { IsUpsert = false },
            token
        );
    }

    public async Task<Domain.Entities.Customer> GetByIdAsync(Guid id)
    {
        var customer = await _customers
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();

        return customer;
    }

    public async Task<IEnumerable<CreateCustomerResponse>> SearchAsync()
    {
        var customers = await _customers
            .Find(Builders<Domain.Entities.Customer>.Filter.Empty)
            .Project(t => new CreateCustomerResponse(t.Id, t.FullName))
            .ToListAsync();

        return customers;
    }
}