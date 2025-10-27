using CashFlow.Customer.Api.Domain.Events;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Customer.Api.Domain.Services;

public interface ICreateCustomer
{
    Task<CreateCustomerResponse> HandleAsync(CreateCustomerRequest request, CancellationToken ct);
}

public class CustomerService : ICreateCustomer
{
    private readonly ILogger<CustomerService> _logger;
    private readonly IMongoCollection<Customer> _customers;
    private readonly IEventBus _eventBus;

    public CustomerService(ILogger<CustomerService> logger, IOptions<MongoDbConfiguration> mongoOptions, IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
        
        var config = mongoOptions.Value;
        var connectionString =
            $"mongodb://{config.Username}:{config.Password}@{config.Host}:{config.Port}/{config.Database}?authSource={config.Username}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(config.Database);
        _customers = database.GetCollection<Customer>("customers");
    }
    
    public async Task<CreateCustomerResponse> HandleAsync(CreateCustomerRequest request, CancellationToken ct)
    {
        var customer = new Customer(request.FullName);
        
        await _customers.InsertOneAsync(customer, cancellationToken: ct);

        var customerEvent = new CustomerCreated(customer.Id, request.FullName);
        
        await _eventBus.PublishAsync(customerEvent, "queuing.customers.created");
        
        _logger.LogInformation("Created customer {FullName} Id: {Id}", customer.FullName,  customer.Id);
        
        return new CreateCustomerResponse(customer.Id, customer.FullName);
    }
}