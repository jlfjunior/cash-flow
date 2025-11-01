using CashFlow.Customer.Api.Application.Requests;
using CashFlow.Customer.Api.Application.Responses;
using CashFlow.Customer.Api.Domain.Events;
using CashFlow.Customer.Api.Infrastructure;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Customer.Api.Application;

public interface ICreateCustomer
{
    Task<CreateCustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct);
}

public class CreateCustomer : ICreateCustomer
{
    private readonly ILogger<CreateCustomer> _logger;
    private readonly IMongoCollection<Domain.Customer> _customers;
    private readonly IEventBus _eventBus;

    public CreateCustomer(ILogger<CreateCustomer> logger, IOptions<MongoDbConfiguration> mongoOptions, IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
        
        var config = mongoOptions.Value;
        var connectionString =
            $"mongodb://{config.Username}:{config.Password}@{config.Host}:{config.Port}/{config.Database}?authSource={config.Username}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(config.Database);
        _customers = database.GetCollection<Domain.Customer>("customers");
    }
    
    public async Task<CreateCustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct)
    {
        var customer = new Domain.Customer(request.FullName);
        
        await _customers.InsertOneAsync(customer, cancellationToken: ct);

        var customerEvent = new CustomerCreated(customer.Id, request.FullName);
        
        await _eventBus.PublishAsync(customerEvent, "queuing.customers.created");
        
        _logger.LogInformation("Created customer. Id: {Id}", customer.Id);
        
        return new CreateCustomerResponse(customer.Id, customer.FullName);
    }
}